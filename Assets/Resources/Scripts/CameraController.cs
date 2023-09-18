using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform m_objectToLookAt;
    [SerializeField]
    private float m_rotationSpeed = 5.0f;
    [SerializeField]
    private Vector2 m_clampingXRotationValues = Vector2.zero;
    [SerializeField]
    private float m_smoothCameraFollow = 2.0f;

    [Header("Scrolling Settings")]
    [SerializeField]
    private float m_scrollSpeed = 2.0f;
    [SerializeField]
    private float m_closestCamDist = 2.0f;
    [SerializeField]
    private float m_farthestCamDist = 16.0f;
    [SerializeField]
    private float m_farthestCamDistFOV = 10.0f;
    [SerializeField]
    private float m_closestCamDistFOV = 60.0f;
    [SerializeField]
    private float m_scrollSmoothDampTime = 0.7f;

    [Header("Camera Obstruction")]
    [SerializeField]
    private float m_lerpInfrontObstructionSpeed = 16.0f;
    [SerializeField]
    private float m_floorObstructionRaycastHeight = 2.0f;

    private Vector3 m_cameraVelocity = Vector3.zero;

    private const float SCROLL_POS_SAFE_THRESHOLD = 2.5f;
    private const float SCROLL_FOV_SLOW_TRANSITION = 55.0f;

    private float TemporaryOffset { get; set; }
    private float DesiredOffset { get;  set; }
    private float m_previousScrollDelta = 0.0f;

    private bool m_cameraIsObstructedByObject = false;
    private bool m_cameraIsObstructedByFloor = false;

    private void Awake()
    {
        DesiredOffset = m_farthestCamDist;
        TemporaryOffset = m_farthestCamDist;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateFollowPlayer();
        UpdateHorizontalRotations();
        UpdateVerticalRotations();
    }

    void FixedUpdate()
    {
        FixedUpdateObjectObstruction();
        FixedUpdateFloorObstruction();
    }

    private void LateUpdate()
    {
        UpdateCameraScroll();
        UpdateFOV();
    }

    private void UpdateFollowPlayer()
    {
        Vector3 targetPosition = m_objectToLookAt.position - transform.forward * TemporaryOffset;
        Vector3 smoothLerpedToTarget = Vector3.Lerp(transform.position, targetPosition, m_smoothCameraFollow * Time.deltaTime);

        // Keep the Y raw so that the camera stays on the same level as the player move and jumps with the player
        smoothLerpedToTarget.y = transform.position.y;

        // Smoothly interpolate the camera position towards the target position
        transform.position = smoothLerpedToTarget;

        // Look at the target
        transform.LookAt(m_objectToLookAt);
    }

    private void UpdateHorizontalRotations()
    {
        float currentAngleX = Input.GetAxis("Mouse X") * m_rotationSpeed;
        transform.RotateAround(m_objectToLookAt.position, m_objectToLookAt.up, currentAngleX);
    }

    private void UpdateVerticalRotations()
    {
        float currentAngleY = Input.GetAxis("Mouse Y") * m_rotationSpeed;
        float eulersAngleX = transform.rotation.eulerAngles.x;

        float comparisonAngle = eulersAngleX + currentAngleY;

        comparisonAngle = ClampAngle(comparisonAngle);

        if ((currentAngleY < 0 && comparisonAngle < m_clampingXRotationValues.x)
            || (currentAngleY > 0 && comparisonAngle > m_clampingXRotationValues.y))
        {
            //Debug.Log("Clamping angle");
            return;
        } 

        transform.RotateAround(m_objectToLookAt.position, transform.right, currentAngleY);
    }

    private float ClampAngle(float angle)
    {
        if (angle > 180)
        {
            angle -= 360;
        }
        return angle;
    }

    private void UpdateCameraScroll()
    {
        float scrollDelta = Input.mouseScrollDelta.y;

        if (Mathf.Approximately(scrollDelta, 0f))
        {
            return;
        }

        // Prevent a glitch with some mouse (mine) of inputting a negative scroll on a positive one 
        if (Mathf.Sign(m_previousScrollDelta) != Mathf.Sign(scrollDelta))
        {
            m_previousScrollDelta = scrollDelta;
            return;
        }

        // Scroll faster at smaller FOV to avoid the impression of slowness the change of FOV gives
        float smallFOVSpeed = 1.0f;
        if (transform.GetComponent<Camera>().fieldOfView < SCROLL_FOV_SLOW_TRANSITION)
        {
            smallFOVSpeed = 4.0f;
        }

        // Calculate the desired camera offset based on the scroll input
        Vector3 desiredCamTranslation = transform.forward * scrollDelta * smallFOVSpeed * m_scrollSpeed;

        // Calculate the new camera position
        Vector3 newPosition = transform.position + desiredCamTranslation;

        // Return if the new position is not within the desired range
        if (IsPosWithinScrollRange(newPosition) == false)
        {
            return;
        }

        // Else apply the camera offset
        // https://docs.unity3d.com/ScriptReference/Vector3.SmoothDamp.html
        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref m_cameraVelocity, m_scrollSmoothDampTime, Mathf.Infinity, Time.deltaTime);

        TemporaryOffset = Vector3.Distance(newPosition, m_objectToLookAt.position);
        m_previousScrollDelta = scrollDelta;
    }

    private void UpdateFOV()
    {
        float currentDistance = Vector3.Distance(transform.position, m_objectToLookAt.position);
        float distancePercent = currentDistance / m_farthestCamDist;

        float newFOV = Mathf.Lerp(m_closestCamDistFOV, m_farthestCamDistFOV, distancePercent);
        transform.GetComponent<Camera>().fieldOfView = newFOV;
    }

    private void FixedUpdateObjectObstruction()
    {
        // Bit shift the index of the layer (8) to get a bit mask
        // Add static object layer 8 (static objects)
        int layerMask = 1 << 8;

        RaycastHit hit;

        // Does the ray intersect any objects excluding the player layer
        Vector3 playerToCamVect = transform.position - m_objectToLookAt.position;
        float distance = playerToCamVect.magnitude;

        if (Physics.Raycast(m_objectToLookAt.position, playerToCamVect, out hit, distance, layerMask))
        {
            if (m_cameraIsObstructedByObject == false)
            {
                m_cameraIsObstructedByObject = true;
                DesiredOffset = Vector3.Distance(transform.position, m_objectToLookAt.position);
            }

            Vector3 hitPoint = hit.point;

            DrawCrosshair(hitPoint);

            Debug.DrawRay(m_objectToLookAt.position, playerToCamVect.normalized * hit.distance, Color.red);

            LerpToPoint(hitPoint);
            return;
        }

        // Draw non-collided rays in green
        Debug.DrawRay(m_objectToLookAt.position, playerToCamVect, Color.green);

        if (m_cameraIsObstructedByObject == false)
        {
            return;
        }

        TemporaryOffset = DesiredOffset;
        m_cameraIsObstructedByObject = false;
    }

    private void FixedUpdateFloorObstruction()
    {
        // Bit shift the index of the layer (7) to get a bit mask
        // Add static object layer 7 (floor objects)
        int layerMask = 1 << 7;

        RaycastHit hit;

        // Does the ray intersect the floor and the camera
        Vector3 cameraDownVector = transform.position;
        cameraDownVector.y += m_floorObstructionRaycastHeight;
        Vector3 downVectToCam = transform.position - cameraDownVector;
        float distance = downVectToCam.magnitude;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, m_floorObstructionRaycastHeight, layerMask))
        {
            if (m_cameraIsObstructedByFloor == false)
            {
                m_cameraIsObstructedByFloor = true;
                DesiredOffset = Vector3.Distance(transform.position, cameraDownVector);
            }

            Vector3 hitPoint = hit.point;

            DrawCrosshair(hitPoint);

            Debug.DrawRay(transform.position, downVectToCam.normalized * hit.distance, Color.red);

            hitPoint.y += m_floorObstructionRaycastHeight;
            //DrawCrosshair(hitPoint);

            LerpToPoint(hitPoint);
            return;
        }

        // Draw non-collided rays in green
        Debug.DrawRay(transform.position, downVectToCam, Color.blue);

        if (m_cameraIsObstructedByFloor == false)
        {
            return;
        }

        TemporaryOffset = DesiredOffset;
        m_cameraIsObstructedByFloor = false;
    }

    private static void DrawCrosshair(Vector3 hitPoint)
    {
        // Draw crosshair at hit point position
        float crosshairSize = 0.1f; // Adjust this value to control the size of the crosshair
        Color crosshairColor = Color.yellow; // Change this value to set the color of the crosshair
                                             // Draw horizontal line
        Debug.DrawLine(hitPoint - Vector3.right * crosshairSize, hitPoint + Vector3.right * crosshairSize, crosshairColor);
        // Draw vertical line
        Debug.DrawLine(hitPoint - Vector3.up * crosshairSize, hitPoint + Vector3.up * crosshairSize, crosshairColor);
    }

    private void LerpToPoint(Vector3 hitPoint)
    {
        Vector3 lerpedHitPoint = Vector3.Lerp(transform.position, hitPoint, Time.deltaTime * m_lerpInfrontObstructionSpeed);
        transform.SetPositionAndRotation(lerpedHitPoint, transform.rotation);
        TemporaryOffset = Vector3.Distance(transform.position, m_objectToLookAt.position);
    }

    private bool IsPosWithinScrollRange(Vector3 position)
    {
        // Check if the new position is within the desired scroll range
        float distance = Vector3.Distance(position, m_objectToLookAt.position);

        // Round to two decimal places
        distance = Mathf.Round(distance * 100f) / 100f;

        bool isWithinClosestRange = distance >= m_closestCamDist + SCROLL_POS_SAFE_THRESHOLD;
        bool isWithinFarthestRange = distance <= m_farthestCamDist - SCROLL_POS_SAFE_THRESHOLD;
        bool isWithinRange = isWithinClosestRange && isWithinFarthestRange;
        
        return isWithinRange;
    }
}