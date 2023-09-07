using UnityEngine;

public class FreeState : CharacterState
{
    public override void OnEnter()
    {
        Debug.Log("Enter state: Free State");
    }

    public override void OnUpdate()
    {
    }

    public override void OnFixedUpdate()
    {
        Vector3 newDirection = Vector3.zero;
        //var vectorOnFloorFront = Vector3.ProjectOnPlane(m_stateMachine.Camera.transform.forward, Vector3.up);
        //var vectorOnFloorSide = Vector3.ProjectOnPlane(m_stateMachine.Camera.transform.right, Vector3.up);

        //vectorOnFloorFront.Normalize();
        //vectorOnFloorSide.Normalize();

        if (Input.GetKey(KeyCode.W))
        {
            Debug.Log("W Forward Pressed");
            Vector3 vectorOnFloorFront = Vector3.ProjectOnPlane(m_stateMachine.Camera.transform.forward, Vector3.up);
            newDirection += vectorOnFloorFront * m_stateMachine.AccelerationValue;
            //m_stateMachine.RB.AddForce(vectorOnFloorFront * m_stateMachine.AccelerationValue, ForceMode.Acceleration);
        }
        if (Input.GetKey(KeyCode.S))
        {
            Vector3 vectorOnFloorFront = Vector3.ProjectOnPlane(m_stateMachine.Camera.transform.forward, Vector3.up);
            float slownValue = CharacterControllerStateMachine.SLOWN_DEPLACEMENT;
            newDirection -= vectorOnFloorFront * m_stateMachine.AccelerationValue * slownValue;
            //m_stateMachine.RB.AddForce(-vectorOnFloorFront * m_stateMachine.AccelerationValue * slownValue, ForceMode.Acceleration);
        }
        if (Input.GetKey(KeyCode.A))
        {
            float slownValue = CharacterControllerStateMachine.SLOWN_DEPLACEMENT;
            Vector3 vectorOnFloorSide = Vector3.ProjectOnPlane(m_stateMachine.Camera.transform.right, Vector3.up);
            newDirection -= vectorOnFloorSide * m_stateMachine.AccelerationValue * slownValue;
            //m_stateMachine.RB.AddForce(-vectorOnFloorSide * m_stateMachine.AccelerationValue * slownValue, ForceMode.Acceleration);
        }
        if (Input.GetKey(KeyCode.D))
        {
            float slownValue = CharacterControllerStateMachine.SLOWN_DEPLACEMENT;
            Vector3 vectorOnFloorSide = Vector3.ProjectOnPlane(m_stateMachine.Camera.transform.right, Vector3.up);
            newDirection += vectorOnFloorSide * m_stateMachine.AccelerationValue * slownValue;
            //m_stateMachine.RB.AddForce(vectorOnFloorSide * m_stateMachine.AccelerationValue * slownValue, ForceMode.Acceleration);
        }

        newDirection.Normalize();
        m_stateMachine.RB.AddForce(newDirection * Time.fixedDeltaTime, ForceMode.Acceleration);

        if (!Input.anyKey)
        {
            m_stateMachine.RB.velocity = Vector3.zero;
        }
        if (m_stateMachine.RB.velocity.magnitude > m_stateMachine.MaxVelocity)
        {
            m_stateMachine.RB.velocity = m_stateMachine.RB.velocity.normalized;
            m_stateMachine.RB.velocity *= m_stateMachine.MaxVelocity;
        }
    }

    public override void OnExit()
    {
        Debug.Log("Exit state: Free state");
    }

    public override bool CanEnter()
    {
        // I can only enter the free state only if I touch the ground
        return m_stateMachine.IsInContactWithFloor();
    }

    public override bool CanExit()
    {
        return true;
    }
}
