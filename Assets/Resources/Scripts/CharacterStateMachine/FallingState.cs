using Unity.VisualScripting;
using UnityEngine;

public class FallingState : CharacterState
{
    private float m_fallingvelocity = 0.0f;
    public override void OnEnter()
    {
        Debug.Log("Enter State: Falling state");
        m_stateMachine.UpdateAnimation(this);
    }

    public override void OnExit()
    {
        Debug.Log("Exit state: Falling state");
        if(Absolute(m_fallingvelocity) > 12.0f)
        m_stateMachine.UpdateAnimation(this);
    }

    public override void OnFixedUpdate()
    {

    }

    public override void OnUpdate()
    {
        Debug.Log("Current falling velocity: " + m_stateMachine.RB.velocity.y);
        m_fallingvelocity = m_stateMachine.RB.velocity.y;
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
