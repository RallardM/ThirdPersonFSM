using Unity.VisualScripting;
using UnityEngine;

public class FallingState : CharacterState
{
    private float m_fallingvelocity = 0.0f;
    private const float MAX_FALLING_VELOCITY_BEFORE_DAMAGE = 12.0f;
    public override void OnEnter()
    {
        Debug.Log("Enter State: Falling state");
        m_stateMachine.UpdateAnimation(this);
    }

    public override void OnExit()
    {
        Debug.Log("Exit State: Falling state");
        m_stateMachine.UpdateAnimation(this);
    }

    public override void OnFixedUpdate()
    {

    }

    public override void OnUpdate()
    {
        if (Mathf.Abs(m_stateMachine.RB.velocity.y) < m_fallingvelocity)
        {
            return;
        }

        // Register velocity if it is greater than the previous one
        m_fallingvelocity = Mathf.Abs(m_stateMachine.RB.velocity.y);
        //Debug.Log("Current falling velocity: " + Mathf.Abs(m_stateMachine.RB.velocity.y));
    }

    public override bool CanEnter(CharacterState currentState)
    {
        return m_stateMachine.IsInContactWithFloor() == false && currentState is not JumpState;
    }

    public override bool CanExit()
    {
        if (m_stateMachine.IsInContactWithFloor())
        {
            Debug.Log("Can exit falling state, is in contact with ground.");

            Debug.Log("Exit state: Falling state");
            if (Mathf.Abs(m_fallingvelocity) > MAX_FALLING_VELOCITY_BEFORE_DAMAGE)
            {
                Debug.Log("Player took damage from falling : " + (int)m_fallingvelocity);
                m_stateMachine.TakeDamage((int)m_fallingvelocity);
            }
            else
            {
                Debug.Log("Player didn't took damage from falling.");
                m_stateMachine.UpdateAnimation(this);
            }

            m_fallingvelocity = 0.0f;
            return m_stateMachine.IsInContactWithFloor();
        }

        return false;
    }
}
