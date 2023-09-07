using UnityEngine;

public class FreeState : CharacterState
{
    public override void OnEnter()
    {
    }

    public override void OnUpdate()
    {
    }

    public override void OnFixedUpdate()
    {
        var vectorOnFloor = Vector3.ProjectOnPlane(m_stateMachine.Camera.transform.forward, Vector3.up);
        vectorOnFloor.Normalize();

        if (Input.GetKey(KeyCode.W))
        {
            m_stateMachine.RB.AddForce(vectorOnFloor * m_stateMachine.AccelerationValue, ForceMode.Acceleration);
        }
        if (Input.GetKey(KeyCode.S))
        {
            float slownValue = CharacterControllerStateMachine.SLOWN_DEPLACEMENT;
            m_stateMachine.RB.AddForce(-vectorOnFloor * m_stateMachine.AccelerationValue * slownValue, ForceMode.Acceleration);
        }
        if (Input.GetKey(KeyCode.A))
        {
            float slownValue = CharacterControllerStateMachine.SLOWN_DEPLACEMENT;
            m_stateMachine.RB.AddForce(-m_stateMachine.Camera.transform.right * m_stateMachine.AccelerationValue * slownValue, ForceMode.Acceleration);
        }
        if (Input.GetKey(KeyCode.D))
        {
            float slownValue = CharacterControllerStateMachine.SLOWN_DEPLACEMENT;
            m_stateMachine.RB.AddForce(m_stateMachine.Camera.transform.right * m_stateMachine.AccelerationValue * slownValue, ForceMode.Acceleration);
        }
        if (!Input.anyKey)
        {
            m_stateMachine.RB.velocity = Vector3.zero;
        }
        if (m_stateMachine.RB.velocity.magnitude > m_stateMachine.MaxVelocity)
        {
            m_stateMachine.RB.velocity = m_stateMachine.RB.velocity.normalized;
            m_stateMachine.RB.velocity *= m_stateMachine.MaxVelocity;
        }

        Debug.Log(m_stateMachine.RB.velocity.magnitude);
    }

    public override void OnExit()
    {
    }
}
