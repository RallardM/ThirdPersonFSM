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
    [SerializeField]
    private float m_smoothCameraUnobstruct = 4.0f;

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
    //private float m_lerpInfrontObstructionSpeed = 16.0f;

    private Vector3 m_cameraVelocity = Vector3.zero;
    //private Vector3 m_lastObstrutionDistance = Vector3.zero;
    private Vector3 m_currentObstrutionDistance = Vector3.zero;
    private Vector3 m_playerToCamVect = Vector3.zero;

    private RaycastHit ObjectObstructHit { get; set; } = new RaycastHit();


    private const float SCROLL_POS_SAFE_THRESHOLD = 2.5f;
    private const float SCROLL_FOV_SLOW_TRANSITION = 55.0f;

    private float CurrentScrollDistance { get; set; }
    private float DesiredDistance { get; set; }
    private float m_previousScrollDelta = 0.0f;

    [SerializeField]
    private bool m_cameraIsObstructed = false;

    private void Awake()
    {
        //DesiredOffset = m_farthestCamDist;
        CurrentScrollDistance = m_farthestCamDist;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateFollowPlayer();
        UpdateHorizontalRotations();
        UpdateVerticalRotations();
        //UpdateFloorRaycastLength();
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
        float cameraSmoothDiplacement = 0.0f;
        if (m_cameraIsObstructed)
        {
            cameraSmoothDiplacement = m_smoothCameraUnobstruct;
        }
        else
        {
            cameraSmoothDiplacement = m_smoothCameraFollow;
        }

        // Use the current offset of the camera to follow the player position
        Vector3 targetPosition = m_objectToLookAt.position - transform.forward * CurrentScrollDistance;
        Vector3 smoothLerpedToTarget = Vector3.Lerp(transform.position, targetPosition, cameraSmoothDiplacement * Time.deltaTime);

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
        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref m_cameraVelocity, m_scrollSmoothDampTime, Mathf.Infinity, Time.deltaTime);

        CurrentScrollDistance = Vector3.Distance(newPosition, m_objectToLookAt.position);
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

        // Does the raycast intersect any objects excluding the player layer
        Vector3 playerToCamObstructionVect = transform.position - m_objectToLookAt.position;
        m_playerToCamVect = playerToCamObstructionVect;

        float distance = 0.0f;
        //Debug.Log("DesiredOffset : " + DesiredOffset);
        if (DesiredDistance == 0.0f)
        {
            distance = playerToCamObstructionVect.magnitude;
        }
        else
        {
            distance = DesiredDistance;
        }
        

        if (Physics.Raycast(m_objectToLookAt.position, playerToCamObstructionVect, out ObjectObstructHitTemp, distance, layerMask)) // Objects obstruction
        {
            //Debug.Log("Camera obstructed");
            m_currentObstrutionDistance = playerToCamObstructionVect;
            ObjectObstructHit = ObjectObstructHitTemp;
            if (m_cameraIsObstructed == false)
            {
                //Debug.Log("Camera offset registered");
                DesiredDistance = distance;
                //m_lastObstrutionDistance = playerToCamObstructionVect;
                m_cameraIsObstructed = true;
            }

            return;
        }

        if(m_cameraIsObstructed == false)
        {
            return;
        }

        //Debug.Log("Camera not obstructed, CurrentOffset : " + CurrentOffset + " DesiredOffset : " + DesiredOffset);
        CurrentScrollDistance = DesiredDistance;
        m_cameraIsObstructed = false;
    }

    private void UpdateObstructionRaycasts()
    {
        if (m_cameraIsObstructed)
        {
            // Object obstruction red raycast
            Debug.DrawRay(m_objectToLookAt.position, m_currentObstrutionDistance.normalized * ObjectObstructHit.distance, Color.red);

            return; // Comment to see both green and red
        }

        // Object obstruction green raycast
        Debug.DrawRay(m_objectToLookAt.position, m_playerToCamVect, Color.green);
    }

    private void UpdateObstructionOffsetPosition()
    {
        //Debug.Log("m_objectObstructionRaycastHit lateUpdate" + ObjectObstructHit.point);
        if (m_cameraIsObstructed && ObjectObstructHit.point != Vector3.zero)
        {
            //Debug.Log("Lerp to hit point : " + ObjectObstructHit.point);
            DrawCrosshair(ObjectObstructHit.point);
            LerpToPoint();
            //m_playerToCamObstructionVect = Vector3.zero;
            //ObjectObstructHit = new RaycastHit();
            return;
        }

        //if (m_cameraIsObstructed == false)
        //{
        //    return;
        //}

        //CurrentOffset = DesiredOffset;
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

    private void LerpToPoint()
    {
        Vector3 camPlayerVector = m_objectToLookAt.position - transform.forward;
        Vector3 hitPlayerVector = m_objectToLookAt.position - ObjectObstructHit.point;

        float currentCamPlayerDistance = Vector3.Distance(m_objectToLookAt.position, transform.forward);
        float desiredHitPlayerDistance = Vector3.Distance(m_objectToLookAt.position, ObjectObstructHit.point);

        if (currentCamPlayerDistance < desiredHitPlayerDistance 
            || currentCamPlayerDistance <= m_closestCamDist)
        {
            
            return;
        }

        Vector3 newPosition = camPlayerVector.normalized * m_closestCamDist;
        CurrentScrollDistance = Vector3.Distance(hitPlayerVector, m_objectToLookAt.position);
        //CurrentOffset = Vector3.Distance(newPosition, m_objectToLookAt.position);
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