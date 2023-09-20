using UnityEngine;

public class AttackState : CharacterState
{
    private const float STATE_EXIT_TIMER = 0.18f;
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

        // Rotate the player's mesh toward the camera direction where the player throws the punch
        Vector3 cameraForward = Vector3.ProjectOnPlane(m_stateMachine.Camera.transform.forward, Vector3.up).normalized;
        cameraForward = cameraForward.normalized;
        Vector3 vectOnFloor = Vector3.ProjectOnPlane(cameraForward, Vector3.up).normalized;
        Quaternion meshRotation = Quaternion.LookRotation(vectOnFloor, Vector3.up);
        float interpolationSpeed = 16.0f;
        m_stateMachine.RB.rotation = Quaternion.Slerp(m_stateMachine.RB.rotation, meshRotation, interpolationSpeed * Time.deltaTime);
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
