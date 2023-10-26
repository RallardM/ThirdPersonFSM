using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    [SerializeField]
    private CharacterAudioController m_audioController;

    [SerializeField]
    protected bool m_canHit;

    [SerializeField]
    protected bool m_canReceiveHit;

    [SerializeField]
    protected EAgentType m_agentType = EAgentType.Count;

    [SerializeField]
    protected List<EAgentType> m_affectedAgentTypes = new List<EAgentType>();

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
        Debug.Log(gameObject.name + " got hit by " + otherHitBox.gameObject.name);
    }
}

public enum EAgentType
{
    Ally,
    Enemy,
    Neutral,
    Count
}
