using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventDispatcher : MonoBehaviour
{
    private CharacterControllerStateMachine m_stateMachineRef;

    private void Awake()
    {
        m_stateMachineRef = GetComponentInChildren<CharacterControllerStateMachine>();
    }

    public void ActivateHitBox()
    {
        m_stateMachineRef.OnEnableAttackHitBox(true);
    }

    public void DeactivateHitBox() 
    {
        m_stateMachineRef.OnEnableAttackHitBox(false);
    }
}
