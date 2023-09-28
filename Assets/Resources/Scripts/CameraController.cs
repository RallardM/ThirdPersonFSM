using System;
using Unity.Burst.CompilerServices;
using Unity.Mathematics;
using Unity.VisualScripting;
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
    private float m_currentFloorObstructionRaycastLength = 1.0f;
    [SerializeField]
    private float m_floorRaycastMaxLength = 2.0f;
    [SerializeField]
    private float m_floorRaycastMinLength = 1.0f;

    private Vector3 m_cameraVelocity = Vector3.zero;
    private Vector3 m_playerToCamObstructionVect = Vector3.zero;
    private Vector3 m_downVectToCamFloorObstructionDetector;
    private RaycastHit ObjectObstructHit { get; set; } = new RaycastHit();
    private RaycastHit FloorObstructHit { get; set; } = new RaycastHit();

    private const float SCROLL_POS_SAFE_THRESHOLD = 2.5f;
    private const float SCROLL_FOV_SLOW_TRANSITION = 55.0f;

    private float CurrentOffset { get; set; }
    private float DesiredOffset { get; set; }
    private float m_previousScrollDelta = 0.0f;
    //private float m_floorDetectLengthScalingFactor = 0.1f;

    private const uint DECUPLE = 10;
    private bool m_cameraIsObstructed = false;

    private void Awake()
    {
        DesiredOffset = m_farthestCamDist;
        CurrentOffset = m_farthestCamDist;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateFollowPlayer();
        UpdateHorizontalRotations();
        UpdateVerticalRotations();
        UpdateFloorRaycastLength();
    }

    void FixedUpdate()
    {
        CheckIfCameraObstructed();
        UpdateObstructionRaycasts();
    }

    private void LateUpdate()
    {
        UpdateCameraScroll();
        UpdateFOV();
        UpdateObstructionOffsetPosition();
    }

    private void UpdateFollowPlayer()
    {
        // Use the current offset of the camera to follow the player position
        Vector3 targetPosition = m_objectToLookAt.position - transform.forward * CurrentOffset;
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

        CurrentOffset = Vector3.Distance(newPosition, m_objectToLookAt.position);
        m_previousScrollDelta = scrollDelta;
    }

    private void UpdateFOV()
    {
        float currentDistance = Vector3.Distance(transform.position, m_objectToLookAt.position);
        float distancePercent = currentDistance / m_farthestCamDist;

        float newFOV = Mathf.Lerp(m_closestCamDistFOV, m_farthestCamDistFOV, distancePercent);
        transform.GetComponent<Camera>().fieldOfView = newFOV;
    }

    private void CheckIfCameraObstructed()
    {
        // Bit shift the index of the layer (8) to get a bit mask
        // Add static object layer 8 (static objects)
        int layerMask = 1 << 8;
        // Add layer 7 (floor objects)
        layerMask |= 1 << 7;

        RaycastHit ObjectObstructHitTemp = default;
        //RaycastHit FloorObstructHitTemp = default;

        // Does the raycast intersect any objects excluding the player layer
        m_playerToCamObstructionVect = transform.position - m_objectToLookAt.position;
        float distance = m_playerToCamObstructionVect.magnitude;

        if (Physics.Raycast(m_objectToLookAt.position, m_playerToCamObstructionVect, out ObjectObstructHitTemp, distance, layerMask)) // Objects obstruction
            //|| Physics.Raycast(transform.position, Vector3.down, out FloorObstructHitTemp, m_floorObstructionRaycastHeight, layerMask)) // Floor obstruction
        {
            ObjectObstructHit = ObjectObstructHitTemp;
            //FloorObstructHit = FloorObstructHitTemp;
            //Debug.Log("m_objectObstructionRaycastHit" + ObjectObstructHit.point);
            //Debug.Log("m_floorObstructionRaycastHit" + m_floorObstructionRaycastHit.point);
            if (m_cameraIsObstructed == false)
            {
                Debug.Log("Camera offset registered");
                DesiredOffset = Vector3.Distance(transform.position, m_objectToLookAt.position);
                m_cameraIsObstructed = true;
            }

            // Register the down raycast values at the current position
            Vector3 cameraDownVector = transform.position;
            cameraDownVector.y += m_currentFloorObstructionRaycastLength;
            m_downVectToCamFloorObstructionDetector = transform.position - cameraDownVector;

            return;
        }

        m_cameraIsObstructed = false;
    }

    private void UpdateObstructionRaycasts()
    {
        if (m_cameraIsObstructed)
        {
            // Object obstruction red raycast
            Debug.DrawRay(m_objectToLookAt.position, m_playerToCamObstructionVect.normalized * ObjectObstructHit.distance, Color.red);

            // Floor obstruction red raycast
            Debug.DrawRay(transform.position, m_downVectToCamFloorObstructionDetector.normalized * FloorObstructHit.distance, Color.red);
            return;
        }

        // Object obstruction green raycast
        Debug.DrawRay(m_objectToLookAt.position, m_playerToCamObstructionVect, Color.green);

        // Floor obstruction blue raycast
        Debug.DrawRay(transform.position, m_downVectToCamFloorObstructionDetector, Color.blue);
    }

    private void UpdateObstructionOffsetPosition()
    {
        //Debug.Log("m_objectObstructionRaycastHit lateUpdate" + ObjectObstructHit.point);
        if (m_cameraIsObstructed && ObjectObstructHit.point != Vector3.zero)
        {
            //Debug.Log("Lerp to hit point : " + ObjectObstructHit.point);
            LerpToPoint(ObjectObstructHit.point);
            ResetObstructionVariables();
            return;
        }
        else if(m_cameraIsObstructed && FloorObstructHit.point != Vector3.zero)
        {
            Debug.Log("Lerp to hit point : " + FloorObstructHit.point);
            LerpToPointFaster(FloorObstructHit.point);
            ResetObstructionVariables();
            return;
        }

        if (m_cameraIsObstructed == false)
        {
            return;
        }

        CurrentOffset = DesiredOffset;
    }

    private void ResetObstructionVariables()
    {
        m_playerToCamObstructionVect = Vector3.zero;
        m_downVectToCamFloorObstructionDetector = Vector3.zero;
        ObjectObstructHit = new RaycastHit();
        FloorObstructHit = new RaycastHit();
    }

    // Visual debug
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
        CurrentOffset = Vector3.Distance(transform.position, m_objectToLookAt.position);
    }

    private void LerpToPointFaster(Vector3 hitPoint)
    {
        Vector3 lerpedHitPoint = Vector3.Lerp(transform.position, hitPoint, Time.deltaTime * m_lerpInfrontObstructionSpeed * DECUPLE);
        transform.SetPositionAndRotation(lerpedHitPoint, transform.rotation);
        CurrentOffset = Vector3.Distance(transform.position, m_objectToLookAt.position);
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

    private void UpdateFloorRaycastLength()
    {
        float distance = Vector3.Distance(transform.position, m_objectToLookAt.position);
        float newRaycastLength = Mathf.Lerp(m_floorRaycastMinLength, m_floorRaycastMaxLength, distance / m_farthestCamDist);

        m_currentFloorObstructionRaycastLength = newRaycastLength;
    }
}