using UnityEngine;

public class AttackState : CharacterState
{
    private const float STATE_EXIT_TIMER = 0.2f;
    private float m_currentStateTimer = 0.0f;

    public override void OnEnter()
    {
        Debug.Log("Enter State: Attacking state");

        m_stateMachine.UpdateAttackAnimation();
        m_currentStateTimer = STATE_EXIT_TIMER;
    }

    public override void OnExit()
    {
        Debug.Log("Exit state: Attacking state");
    }

    public override void OnFixedUpdate()
    {

    }

    public override void OnUpdate()
    {
        m_currentStateTimer -= Time.deltaTime;
    }

    public override bool CanEnter(CharacterState currentState)
    {
        return Input.GetMouseButtonDown(0);
    }

    public override bool CanExit()
    {
        return m_currentStateTimer <= 0.0f;
    }
}
