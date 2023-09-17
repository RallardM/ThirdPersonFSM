using UnityEngine;

public class GettingHitState : CharacterState
{
    public override void OnEnter()
    {
        Debug.Log("Enter State: Getting Hit state");

    }

    public override void OnExit()
    {
        Debug.Log("Exit state: Getting Hit state");
    }

    public override void OnFixedUpdate()
    {

    }

    public override void OnUpdate()
    {
        
    }

    public override bool CanEnter()
    {
        return m_stateMachine.Health < m_stateMachine.PreviousHealth;
    }

    public override bool CanExit()
    {
        return true;
    }
}
