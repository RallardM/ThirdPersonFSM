using UnityEngine;

public class NonGameplayState : CharacterState
{
    public override void OnEnter()
    {
        Debug.Log("Enter State: NonGameplayState");
    }

    public override void OnExit()
    {
        Debug.Log("Exit state: NonGameplayState");
    }

    public override void OnFixedUpdate()
    {

    }

    public override void OnUpdate()
    {

    }

    public override bool CanEnter(IState currentState)
    {
        return m_stateMachine.InNonGameplayState;
    }

    public override bool CanExit()
    {
        return !m_stateMachine.InNonGameplayState;
    }
}
