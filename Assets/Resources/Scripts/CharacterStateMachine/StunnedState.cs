using UnityEngine;

public class StunnedState : CharacterState
{
    public override void OnEnter()
    {
        Debug.Log("Enter State: Stunned state");
        m_stateMachine.UpdateAnimation(this);
    }

    public override void OnExit()
    {
        Debug.Log("Exit state: Stunned state");
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
        if (m_stateMachine.IsStunned) 
        {
            Debug.Log("Can enter Stunned sate  : " + currentState.GetType().Name);
            return true;
        }

        return false;
    }

    public override bool CanExit()
    {
        if (m_stateMachine.IsDead == false && m_stateMachine.IsKeyPressed)
        {
            Debug.Log("Can exit Stunned state : a key is pressed");
            return true;
        }

        return false;
    }
}
