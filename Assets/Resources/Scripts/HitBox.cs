using Cinemachine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    [SerializeField]
    private CharacterAudioController m_audioController;

    [SerializeField]
    private CameraShake m_cameraShake;

    [SerializeField]
    protected bool m_canHit;

    [SerializeField]
    protected bool m_canReceiveHit;

    [SerializeField]
    protected EAgentType m_agentType = EAgentType.Count;

    [SerializeField]
    protected List<EAgentType> m_affectedAgentTypes = new List<EAgentType>();

    private Vector3 m_previousPosition;
    private Rigidbody m_hitboxRigidBody;
    public Vector3 m_globalVelocity;

    void Start()
    {
        if (GetComponent<Rigidbody>() == null)
        {
            return;
        }

        m_hitboxRigidBody = GetComponent<Rigidbody>();
        m_previousPosition = m_hitboxRigidBody.transform.position;
    }

    void Update()
    {
        if (m_hitboxRigidBody == null)
        {
            return;
        }

        //if (Input.GetKeyDown(KeyCode.I))
        //{
        //    if (m_impulseSource == null)
        //    {
        //        return;
        //    }

        //    var intensity = Random.Range(1.0f, 4.0f);
        //    m_impulseSource.GenerateImpulse(intensity);
        //}

        // Since the rigidbody is on the squeletton, we need to compute the velocity ourselves
        Vector3 currentPosition = m_hitboxRigidBody.transform.position;
        Vector3 velocity = (currentPosition - m_previousPosition) / Time.deltaTime;
        m_previousPosition = currentPosition;
        m_globalVelocity = m_hitboxRigidBody.transform.InverseTransformDirection(velocity);
    }

    protected void OnTriggerEnter(Collider collider)
    {
        Debug.Log("Collider enter : " + collider.name);
        var otherHitBox = collider.GetComponent<HitBox>();
        if (otherHitBox == null) return;

        // Other collider else is an HitBox
        if (CanHitOther(otherHitBox))
        {
            Debug.Log("Can hit other : " + collider.name);
            VFXManager.GetInstance().InstantiateVFX(EVFX_Type.Hit, collider.ClosestPoint(transform.position));
            m_audioController.PlaySound(ESoundType.Slap);
            otherHitBox.GetHit(this);
        }
    }

    protected bool CanHitOther(HitBox otherHitBox)
    {
        return m_canHit && 
            otherHitBox.m_canReceiveHit && 
            m_affectedAgentTypes.Contains(otherHitBox.m_agentType);
    }

    protected void GetHit(HitBox otherHitBox)
    {
        //Debug.Log(gameObject.name + " got hit by " + otherHitBox.gameObject.GetComponentInParent<EnemyController>().name);
        //Debug.Log(gameObject.name + " got hit by hit box " + otherHitBox.gameObject.name);
        //Debug.Log("Velocity : " + otherHitBox.gameObject.GetComponent<Rigidbody>().velocity);
        Rigidbody rb = otherHitBox.gameObject.GetComponent<Rigidbody>();
        Vector3 localVelocity = rb.velocity;
        Vector3 globalVelocity = rb.transform.TransformDirection(localVelocity);
        //Debug.Log("Global Velocity : " + globalVelocity);
        //Debug.Log("G Velocity : " + otherHitBox.m_globalVelocity);
        //m_impulseSource.GenerateImpulse(otherHitBox.m_globalVelocity);
        m_cameraShake.ShakeCamera(otherHitBox.m_globalVelocity.magnitude);
    }
}

public enum EAgentType
{
    Ally,
    Enemy,
    Neutral,
    Count
}
