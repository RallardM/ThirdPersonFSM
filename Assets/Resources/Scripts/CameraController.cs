using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform m_objectToLookAt;
    [SerializeField]
    private float m_rotationSpeed = 1.0f;
    [SerializeField]
    private Vector2 m_clampingXRotationValues = Vector2.zero;

    private Vector3 m_farthestCamDist = new(0, 74, 300);
    private Vector3 m_farthestCamRotation = new(10, 0, 0);

    private Vector3 m_closestCamDist = new(0, 1, 13);
    private Vector3 m_closestCamRotation = new(0, 0, 0);

    private float m_scrollSpeed = 2.0f;

    float m_farthestCamDistFOV = 3.5f;
    float m_closestCamDistFOV = 24f;

    private void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHorizontalMovements();
        UpdateVerticalMovements();
        UpdateCameraScroll();
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
        if (Input.mouseScrollDelta.y == 0)
        {
            return;
        }

        //TODO: Faire une vérification selon la distance la plus proche ou la plus éloignée
        //Que je souhaite entre ma caméra et mon objet

        Vector3 camTranslation = new(0, 0, 0);

        float currentCamDist = GetDistBetweenTwoVects(transform.position, m_objectToLookAt.position);
        float farthestCamDist = GetDistBetweenTwoVects(transform.position + m_farthestCamDist, m_objectToLookAt.position);
        float closestCamDist = GetDistBetweenTwoVects(transform.position + m_closestCamDist, m_objectToLookAt.position);

        if (currentCamDist > farthestCamDist || currentCamDist < closestCamDist)
        {
            return;
        }
        else
        {
            camTranslation = Vector3.forward;
        }

        //TODO: Lerp plutôt que d'effectuer immédiatement la translation
        //Vector3 scrollPosLerp = Vector3.Lerp(transform.position, transform.position + camTranslation, Time.deltaTime);

        transform.Translate(camTranslation * Input.mouseScrollDelta.y * m_scrollSpeed, Space.Self);
        //transform.Translate(scrollPosLerp, Space.Self);
    }

    private float ClampAngle(float angle)
    {
        if (angle > 180)
        {
            angle -= 360;
        }
        return angle;
    }

    private float GetDistBetweenTwoVects(Vector3 firstVector, Vector3 secondVector)
    {
        return Vector3.Distance(firstVector, secondVector);
    }
}
