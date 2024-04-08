using System.Collections.Generic;
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

        // Since the rigidbody is on the squeletton, we need to compute the velocity ourselves
        Vector3 currentPosition = m_hitboxRigidBody.transform.position;
        Vector3 velocity = (currentPosition - m_previousPosition) / Time.deltaTime;
        m_previousPosition = currentPosition;
        m_globalVelocity = m_hitboxRigidBody.transform.InverseTransformDirection(velocity);
    }

    protected void OnTriggerEnter(Collider collider)
    {
        var otherHitBox = collider.GetComponent<HitBox>();
        if (otherHitBox == null) return;

        // Other collider else is an HitBox
        if (CanHitOther(otherHitBox))
        {
            VFXManager.GetInstance().InstantiateVFX(EVFX_Type.Hit, collider.ClosestPoint(transform.position), m_globalVelocity.magnitude);
            m_audioController.PlaySound(ESoundType.Slap);
        }
    }

    protected bool CanHitOther(HitBox otherHitBox)
    {
        return m_canHit && 
            otherHitBox.m_canReceiveHit && 
            m_affectedAgentTypes.Contains(otherHitBox.m_agentType);
    }
}

public enum EAgentType
{
    Ally,
    Enemy,
    Neutral,
    Count
}
