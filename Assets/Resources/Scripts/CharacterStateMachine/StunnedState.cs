using UnityEngine;

public class StunnedState : CharacterState
{
    public override void OnEnter()
    {
        Debug.Log("Enter State: Stunned state");
    }

    public override void OnExit()
    {
        Debug.Log("Exit state: Stunned state");
        m_stateMachine.StunnedStateEndded();
    }

    public override void OnFixedUpdate()
    {

    }

    public override void OnUpdate()
    {

    }

    public override bool CanEnter(CharacterState currentState)
    {
        return m_stateMachine.IsStunned;
    }

    public override bool CanExit()
    {
        return m_stateMachine.IsDead == false && m_stateMachine.IsKeyPressed;
    }
}
