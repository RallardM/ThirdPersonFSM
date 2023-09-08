using System;
using TMPro;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform m_objectToLookAt;
    [SerializeField]
    private float m_rotationSpeed = 1.0f;
    [SerializeField]
    private Vector2 m_clampingXRotationValues = Vector2.zero;
    [SerializeField]
    private float m_smoothCameraFollow = 2.0f;
    [SerializeField]
    private float m_scrollSpeed = 2.0f;
    [SerializeField]
    private float m_closestCamDist = 1.0f;
    [SerializeField]
    private float m_farthestCamDist = 2.0f;

    private const float SCROLL_POS_SAFE_THRESHOLD = 0.01f;
    
    private float m_cameraDesiredOffset;

    //private float m_farthestCamDistFOV = 10.0f; // TODO
    //private float m_closestCamDistFOV = 60.0f; // TODO

    //private float m_previousPlayerToOffsetDotP = 0f; // TODO

    private void Awake()
    {
        m_cameraDesiredOffset = m_farthestCamDist;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("CameraController::Update, m_cameraDesiredOffset : " + m_cameraDesiredOffset);
        UpdateHorizontalMovements();
        UpdateVerticalMovements();
        UpdateCameraScroll();
        //CheckIfCameraIsInRange();
    }

    void FixedUpdate()
    {
        FixedUpdateTestCameraObstruction();
    }

    private void LateUpdate()
    {
        Vector3 targetPosition = m_objectToLookAt.position - transform.forward * m_cameraDesiredOffset;

        // Smoothly interpolate the camera position towards the target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, m_smoothCameraFollow * Time.deltaTime);

        // Look at the target
        transform.LookAt(m_objectToLookAt);
    }

    private void UpdateHorizontalMovements()
    {
        float currentAngleX = Input.GetAxis("Mouse X") * m_rotationSpeed;
        transform.RotateAround(m_objectToLookAt.position, m_objectToLookAt.up, currentAngleX);
    }

    private void UpdateVerticalMovements()
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

        // Calculate the desired camera offset based on the scroll input
        Vector3 camTranslation = transform.forward * scrollDelta * m_scrollSpeed;

        // Calculate the new camera position
        Vector3 newPosition = transform.position + camTranslation;

        Debug.Log("Is not scrolling not within range");

        // Return if the new position is not within the desired range
        if (IsPosWithinScrollRange(newPosition) == false)
        {
            return;
        }

        Debug.Log("Is scrolling within range");

        // Else apply the camera offset
        transform.Translate(camTranslation, Space.World);
        m_cameraDesiredOffset = Vector3.Distance(transform.position, m_objectToLookAt.position);
    }

    private float ClampAngle(float angle)
    {
        if (angle > 180)
        {
            angle -= 360;
        }
        return angle;
    }

    private void FixedUpdateTestCameraObstruction()
    {
        // Bit shift the index of the layer (8) to get a bit mask
        int layerMask = 1 << 8;

        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        //layerMask = ~layerMask;

        RaycastHit hit;

        // Does the ray intersect any objects excluding the player layer
        var vecteurDiff = transform.position - m_objectToLookAt.position;
        var distance = vecteurDiff.magnitude;

        if (Physics.Raycast(m_objectToLookAt.position, vecteurDiff, out hit, distance, layerMask))
        {
            Debug.DrawRay(m_objectToLookAt.position, vecteurDiff.normalized * hit.distance, Color.red);
            transform.SetPositionAndRotation(hit.point, transform.rotation);
        }
        else
        {
            Debug.DrawRay(m_objectToLookAt.position, vecteurDiff, Color.green);
        }       
    }

    private bool IsPosWithinScrollRange(Vector3 position)
    {
        // Check if the new position is within the desired scroll range
        float distance = Vector3.Distance(position, m_objectToLookAt.position);

        return distance >= m_closestCamDist && distance <= m_farthestCamDist;
    }

    // Source : https://stackoverflow.com/questions/8781990/efficient-way-to-reduce-a-vectors-magnitude-by-a-specific-length
    private Vector3 GetPositionWithinRange(Vector3 position)
    {
        Vector3 newPosition = position;
        float distance = Vector3.Distance(position, m_objectToLookAt.position);
        if (distance < m_closestCamDist)
        {
            newPosition *= (1 - m_closestCamDist + SCROLL_POS_SAFE_THRESHOLD / position.magnitude);
        }
        else if (distance > m_farthestCamDist)
        {
            newPosition *= (1 - m_farthestCamDist - SCROLL_POS_SAFE_THRESHOLD / position.magnitude);
        }

        return newPosition;
    }

    private void CheckIfCameraIsInRange()
    {
        if (IsPosWithinScrollRange(transform.position))
        {
            return;
        }

        //transform.position = GetPositionWithinRange(transform.position);
        //transform.position = Vector3.Lerp(transform.position, GetPositionWithinRange(transform.position), m_smoothCameraFollow * Time.deltaTime);
    }
}