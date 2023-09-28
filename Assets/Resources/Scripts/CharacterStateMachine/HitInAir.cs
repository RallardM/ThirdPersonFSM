using UnityEngine;

public class HitInAir : CharacterState
{
    public override void OnEnter()
    {
        Debug.Log("Enter State: HitInAir state");
        m_stateMachine.UpdateAnimation(this);
    }

    public override void OnExit()
    {
        Debug.Log("Exit State: HitInAir state");
    }

    public override void OnFixedUpdate()
    {

    }

    public override void OnUpdate()
    {

    }

    public override bool CanEnter(CharacterState currentState)
    {
        return currentState is FallingState || currentState is JumpState;
    }

    public override bool CanExit()
    {
        if (m_stateMachine.IsInContactWithFloor())
        {
            Debug.Log("Can exit: hit in air State");
            return true;
        }

        return false;
    }
}
