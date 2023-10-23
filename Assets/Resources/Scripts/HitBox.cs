using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    [SerializeField]
    protected bool m_canHit;
    [SerializeField]
    protected bool m_canReceiveHit;
    [SerializeField]
    protected EAgentType m_agentType = EAgentType.Count;
    [SerializeField]
    protected List<EAgentType> m_affectedAgentTypes = new List<EAgentType>();

    protected void OnCollisionEnter(Collision collision)
    {
        var otherHitBox = collision.collider.GetComponent<HitBox>();
        if (otherHitBox == null) return;

        // Other collider else is an HitBox
        if (CanHitOther(otherHitBox))
        {
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
