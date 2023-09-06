using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform m_objectToLookAt;
    [SerializeField]
    private float m_rotationSpeed = 1.0f;
    [SerializeField]
    private Vector2 m_clampingXRotationValues = Vector2.zero;
    [SerializeField]
    private float m_smoothCameraFollow = 0.125f;

    private float m_cameraDesiredOffset = 2.0f;

    private float m_closestCamDist = 1.1f;
    private float m_farthestCamDist = 1.2f;

    private float m_scrollSpeed = 2.0f;

    private float m_farthestCamDistFOV = 10.0f;
    private float m_closestCamDistFOV = 60.0f;

    private float m_previousPlayerToOffsetDotP = 0f;

    // Update is called once per frame
    void Update()
    {
        UpdateHorizontalMovements();
        UpdateVerticalMovements();
        UpdateCameraScroll();
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

        // Return if the new position is within the desired range
        if (IsWithinScrollRange(newPosition) == false)
        {
            return;
        }

        // Else apply the camera offset
        transform.Translate(camTranslation, Space.World);
        m_cameraDesiredOffset = Vector3.Distance(transform.position, m_objectToLookAt.position);
    }

    private bool IsWithinScrollRange(Vector3 position)
    {
        // Check if the new position is within the desired scroll range
        float distance = Vector3.Distance(position, m_objectToLookAt.position);
        return distance >= m_closestCamDist && distance <= m_farthestCamDist;
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
}