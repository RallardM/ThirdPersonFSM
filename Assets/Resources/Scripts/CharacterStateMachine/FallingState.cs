using UnityEngine;

public class FallingState : CharacterState
{

    public override void OnEnter()
    {
        Debug.Log("Enter State: Falling state");
        m_stateMachine.UpdateAnimation(this);
    }

    public override void OnExit()
    {
        Debug.Log("Exit state: Falling state");
        m_stateMachine.UpdateAnimation(this);
    }

    public override void OnFixedUpdate()
    {

    }

    public override void OnUpdate()
    {

    }

    public override bool CanEnter(CharacterState currentState)
    {
        return m_stateMachine.IsInContactWithFloor() == false && currentState is not JumpState;
    }

    public override bool CanExit()
    {
        return m_stateMachine.IsInContactWithFloor();
    }
}
