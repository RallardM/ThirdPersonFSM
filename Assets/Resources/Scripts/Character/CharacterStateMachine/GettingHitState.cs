using UnityEngine;

public class GettingHitState : CharacterState
{
    private const float STATE_EXIT_TIMER = 0.2f;
    private float m_currentStateTimer = 0.0f;

    public override void OnEnter()
    {
        Debug.Log("Enter State: Getting Hit state");
        m_stateMachine.UpdateHeatlh();
        m_stateMachine.UpdateAnimation(this);
        m_currentStateTimer = STATE_EXIT_TIMER;
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
        m_currentStateTimer -= Time.deltaTime;
    }

    public override bool CanEnter(IState currentState)
    {
        if ((m_stateMachine.Health < m_stateMachine.PreviousHealth)
        && (currentState is not FallingState || currentState is not JumpState))
        {
            Debug.Log("Can enter Getting Hit state: fallingstate " + (currentState is not FallingState) + " jumpstate " + (currentState is not JumpState) + " states : " + (currentState is not FallingState || currentState is not JumpState) + " all :" + ((m_stateMachine.Health < m_stateMachine.PreviousHealth) && (currentState is not FallingState || currentState is not JumpState)));
        }

        return (m_stateMachine.Health < m_stateMachine.PreviousHealth)
        && (currentState is not FallingState || currentState is not JumpState);
    }

    public override bool CanExit()
    {
        return m_currentStateTimer <= 0.0f && m_stateMachine.Health == m_stateMachine.PreviousHealth;
    }
}
