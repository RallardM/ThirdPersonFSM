using System;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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
    private float m_floorObstructionRaycastHeight = 1.0f;

    Vector3 m_cameraVelocity = Vector3.zero;

    private const float SCROLL_POS_SAFE_THRESHOLD = 2.5f;
    private float TemporaryOffset { get; set; }
    private float DesiredOffset { get;  set; }
    private float m_previousScrollDelta = 0.0f;
    

    private bool m_cameraIsObstructed = false;

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
            return;
        }

        transform.RotateAround(m_objectToLookAt.position, transform.right, currentAngleY);
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

        // Calculate the desired camera offset based on the scroll input
        Vector3 desiredCamTranslation = transform.forward * scrollDelta * m_scrollSpeed;

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

    private float ClampAngle(float angle)
    {
        if (angle > 180)
        {
            angle -= 360;
        }
        return angle;
    }

    private void FixedUpdateObjectObstruction()
    {
        // Bit shift the index of the layer (8) to get a bit mask
        // Add static object layer 8 (static objects)
        int layerMask = 1 << 8;

        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        //layerMask = ~layerMask;

        RaycastHit hit;

        // Does the ray intersect any objects excluding the player layer
        Vector3 vecteurDiff = transform.position - m_objectToLookAt.position;
        float distance = vecteurDiff.magnitude;

        if (Physics.Raycast(m_objectToLookAt.position, vecteurDiff, out hit, distance, layerMask))
        {
            if (m_cameraIsObstructed == false)
            {
                m_cameraIsObstructed = true;
                DesiredOffset = Vector3.Distance(transform.position, m_objectToLookAt.position);
            }

            Debug.DrawRay(m_objectToLookAt.position, vecteurDiff.normalized * hit.distance, Color.red);
            Vector3 lerpedHitPoin = Vector3.Lerp(transform.position, hit.point, Time.deltaTime * m_lerpInfrontObstructionSpeed);
            transform.SetPositionAndRotation(lerpedHitPoin, transform.rotation);
            TemporaryOffset = Vector3.Distance(transform.position, m_objectToLookAt.position);
            return;
        }

        Debug.DrawRay(m_objectToLookAt.position, vecteurDiff, Color.green);

        if (m_cameraIsObstructed == false)
        {
            return;
        }

        TemporaryOffset = DesiredOffset;
        m_cameraIsObstructed = false;
    }

    private void FixedUpdateFloorObstruction()
    {
        // Bit shift the index of the layer (8) to get a bit mask
        // Add static object layer 7 (floor)
        int layerMask = 1 << 7;

        RaycastHit hit;

        Vector3 vecteurDiff = transform.position;
        vecteurDiff.y -= m_floorObstructionRaycastHeight;
        float distance = Vector3.Distance(transform.position, vecteurDiff);

        if (Physics.Raycast(transform.position, Vector3.down, out hit, m_floorObstructionRaycastHeight, layerMask))
        {
            Debug.DrawRay(transform.position, Vector3.down * hit.distance, Color.red);

            return;
        }

        Debug.DrawRay(transform.position, Vector3.down, Color.blue);
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

    private void UpdateFOV()
    {
        float currentDistance = Vector3.Distance(transform.position, m_objectToLookAt.position);
        float distancePercent = currentDistance / m_farthestCamDist;

        float newFOV = Mathf.Lerp(m_closestCamDistFOV, m_farthestCamDistFOV, distancePercent);
        transform.GetComponent<Camera>().fieldOfView = newFOV;
    }
}