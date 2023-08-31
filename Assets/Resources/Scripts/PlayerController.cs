using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Camera m_camera;
    private Rigidbody m_rb;

    [SerializeField] private float m_accelerationValue;
    [SerializeField] private float m_maxVelocity;

    private void Awake()
    {
        m_camera = Camera.main;
        m_rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var vectorOnFloor = Vector3.ProjectOnPlane(m_camera.transform.forward, Vector3.up);
        vectorOnFloor.Normalize();

        if (Input.GetKey(KeyCode.W))
        {
            m_rb.AddForce(vectorOnFloor * m_accelerationValue, ForceMode.Acceleration);
        }
        if (m_rb.velocity.magnitude > m_maxVelocity)
        {
            m_rb.velocity = m_rb.velocity.normalized;
            m_rb.velocity *= m_maxVelocity;
        }

        // TODO: Appliquer les deplacements relatifs a la camera dans les 3 autres directions
        // avoir des vitesses de deplacements maximales differentes cersles cotes et vers l'arriere
        // lorsque aucune input est mis, decelerer le personnage rapidement
    }
}
