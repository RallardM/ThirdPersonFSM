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
    private float m_minCamDist = 2.0f;
    [SerializeField]
    private float m_maxCamDist = 40.0f;
    [SerializeField]
    private float m_maxCamDistFOV = 20.0f;
    [SerializeField]
    private float m_minCamDistFOV = 70.0f;
    [SerializeField]
    private float m_scrollSmoothDampTime = 0.7f;
    [SerializeField]
    private bool m_cameraIsObstructed = false;

    private Vector3 m_cameraVelocity = Vector3.zero;
    private Vector3 m_currentObstrutionPosition = Vector3.zero;
    private Vector3 m_currentObstrutionRaycastVect = Vector3.zero;
    private Vector3 m_playerToCamVect = Vector3.zero;
    private Vector3 m_smoothLerpedTowardPlayer = Vector3.zero;

    private const float SCROLL_POS_SAFE_THRESHOLD = 2.5f;
    private const float SCROLL_FOV_SLOW_TRANSITION = 55.0f;

    private float m_previousScrollDelta = 0.0f;
    private float m_currentMaxCamDist = 0.0f;

    private RaycastHit ObjectObstructHit { get; set; } = new RaycastHit();
    private float CurrentScrollDistance { get; set; }
    private float DesiredUnobstructedDistance { get; set; }

    private void Awake()
    {
        //DesiredOffset = m_farthestCamDist;
        CurrentScrollDistance = m_maxCamDist;
        m_currentMaxCamDist = m_maxCamDist;
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

        m_smoothLerpedTowardPlayer = Vector3.zero;

        // Use the current offset of the camera to follow the player position
        Vector3 targetPosition = m_objectToLookAt.position - transform.forward * CurrentScrollDistance;
        m_smoothLerpedTowardPlayer = Vector3.Lerp(transform.position, targetPosition, cameraSmoothDiplacement * Time.deltaTime);

        // Keep the Y raw so that the camera stays on the same level as the player move and jumps with the player
        m_smoothLerpedTowardPlayer.y = transform.position.y;

        UpdateCameraPosition();

        // Look at the target
        transform.LookAt(m_objectToLookAt);
    }

    // All camera translations should be done here
    private void UpdateCameraPosition()
    {
        Vector3 leprFollowCamToObstructCam = Vector3.zero;
        if (m_currentObstrutionPosition != Vector3.zero)
        {
            leprFollowCamToObstructCam = Vector3.Lerp(transform.position, m_currentObstrutionPosition, 0.5f);
        }
        else
        {
            leprFollowCamToObstructCam = m_smoothLerpedTowardPlayer;
        }

        // Smoothly interpolate the camera position towards the new player's position
        transform.position = leprFollowCamToObstructCam;
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
        float distancePercent = currentDistance / m_currentMaxCamDist;

        float newFOV = Mathf.Lerp(m_minCamDistFOV, m_maxCamDistFOV, distancePercent);
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
        if (DesiredUnobstructedDistance == 0.0f)
        {
            distance = playerToCamObstructionVect.magnitude;
        }
        else
        {
            distance = DesiredUnobstructedDistance;
        }

        if (Physics.Raycast(m_objectToLookAt.position, playerToCamObstructionVect, out ObjectObstructHitTemp, distance, layerMask)) // Objects obstruction
        {
            //Debug.Log("Camera obstructed");
            m_currentObstrutionRaycastVect = playerToCamObstructionVect;
            ObjectObstructHit = ObjectObstructHitTemp;
            if (m_cameraIsObstructed == false)
            {
                //Debug.Log("Camera offset registered");
                DesiredUnobstructedDistance = distance;
                m_cameraIsObstructed = true;
            }

            return;
        }

        if(m_cameraIsObstructed == false)
        {
            return;
        }

        m_cameraIsObstructed = false;
    }

    private void UpdateObstructionRaycasts()
    {
        if (m_cameraIsObstructed)
        {
            // Object obstruction red raycast
            Debug.DrawRay(m_objectToLookAt.position, m_currentObstrutionRaycastVect.normalized * ObjectObstructHit.distance, Color.red);
            DrawCrosshair(ObjectObstructHit.point);
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
            float distancePlayerToHitPoint = Vector3.Distance(m_objectToLookAt.position, ObjectObstructHit.point);
            m_currentMaxCamDist = Mathf.Lerp(m_currentMaxCamDist, distancePlayerToHitPoint, Time.deltaTime);
            m_currentObstrutionPosition = ObjectObstructHit.point;
            return;
        }

        m_currentObstrutionPosition = Vector3.zero;
        m_currentMaxCamDist = Mathf.Lerp(m_currentMaxCamDist, m_maxCamDist, Time.deltaTime);
    }

    // Visual debug
    private static void DrawCrosshair(Vector3 hitPoint)
    {
        // Draw crosshair at hit point position
        float crosshairSize = 0.2f; // Adjust this value to control the size of the crosshair
        Color crosshairColor = Color.yellow; // Change this value to set the color of the crosshair
                                             // Draw horizontal line
        Debug.DrawLine(hitPoint - Vector3.right * crosshairSize, hitPoint + Vector3.right * crosshairSize, crosshairColor);
        // Draw vertical line
        Debug.DrawLine(hitPoint - Vector3.up * crosshairSize, hitPoint + Vector3.up * crosshairSize, crosshairColor);
    }

    private bool IsPosWithinScrollRange(Vector3 position)
    {
        // Check if the new position is within the desired scroll range
        float distance = Vector3.Distance(position, m_objectToLookAt.position);

        // Round to two decimal places
        distance = Mathf.Round(distance * 100f) / 100f;

        bool isWithinClosestRange = distance >= m_minCamDist + SCROLL_POS_SAFE_THRESHOLD;
        bool isWithinFarthestRange = distance <= m_currentMaxCamDist - SCROLL_POS_SAFE_THRESHOLD;
        bool isWithinRange = isWithinClosestRange && isWithinFarthestRange;

        return isWithinRange;
    }
}