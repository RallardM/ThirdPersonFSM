using UnityEngine;

public class DeadState : CharacterState
{
    public override void OnEnter()
    {
        Debug.Log("Enter State: dead state");
        m_stateMachine.UpdateAnimation(this);
    }

    public override void OnExit()
    {
    }

    public override void OnFixedUpdate()
    {

    }

    public override void OnUpdate()
    {

    }

    public override bool CanEnter(IState currentState)
    {
        if (m_stateMachine.IsDead)
        {
            Debug.Log("Can enter dead sate  : " + currentState.GetType().Name);
            return true;
        }

        return false;
    }

    public override bool CanExit()
    {
        return false;
    }
}