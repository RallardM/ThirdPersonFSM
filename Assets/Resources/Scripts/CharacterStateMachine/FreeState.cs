using UnityEngine;

public class FreeState : CharacterState
{
    public override void OnEnter()
    {
        //Debug.Log("Enter state: Free State");
    }

    public override void OnUpdate()
    {
    }

    public override void OnFixedUpdate()
    {
        Vector3 newDirection = Vector3.zero;

        if (!Input.anyKey)
        {
            m_stateMachine.RB.velocity = Vector3.zero;
        }
        if (Input.GetKey(KeyCode.W))
        {
            Vector3 vectOnFloorDollyDir = Vector3.ProjectOnPlane(m_stateMachine.Camera.transform.forward, Vector3.up);
            vectOnFloorDollyDir.Normalize();
            newDirection += vectOnFloorDollyDir * m_stateMachine.AccelerationValue;
        }
        if (Input.GetKey(KeyCode.S))
        {
            float slownValue = CharacterControllerStateMachine.SLOWN_DEPLACEMENT;
            Vector3 vectOnFloorDollyDir = Vector3.ProjectOnPlane(m_stateMachine.Camera.transform.forward, Vector3.up);
            vectOnFloorDollyDir.Normalize();
            newDirection -= vectOnFloorDollyDir * m_stateMachine.AccelerationValue * slownValue;
        }
        if (Input.GetKey(KeyCode.A))
        {
            float slownValue = CharacterControllerStateMachine.SLOWN_DEPLACEMENT;
            Vector3 vectOnFloorTruckDir = Vector3.ProjectOnPlane(m_stateMachine.Camera.transform.right, Vector3.up);
            vectOnFloorTruckDir.Normalize();
            newDirection -= vectOnFloorTruckDir * m_stateMachine.AccelerationValue * slownValue;
        }
        if (Input.GetKey(KeyCode.D))
        {
            float slownValue = CharacterControllerStateMachine.SLOWN_DEPLACEMENT;
            Vector3 vectOnFloorTruckDir = Vector3.ProjectOnPlane(m_stateMachine.Camera.transform.right, Vector3.up);
            vectOnFloorTruckDir.Normalize();
            newDirection += vectOnFloorTruckDir * m_stateMachine.AccelerationValue * slownValue;
        }

        // Rotate the player's mesh toward the new input direction
        if (newDirection != Vector3.zero)
        {
            Quaternion meshRotation = Quaternion.LookRotation(newDirection, Vector3.up);
            float interpolationSpeed = 2.0f;
            // Source : https://forum.unity.com/threads/what-is-the-difference-of-quaternion-slerp-and-lerp.101179/
            m_stateMachine.PlayerTransform.rotation = Quaternion.Slerp(m_stateMachine.PlayerTransform.rotation, meshRotation, interpolationSpeed * Time.deltaTime);
        }

        m_stateMachine.RB.AddForce(newDirection, ForceMode.Acceleration);

        if (m_stateMachine.RB.velocity.magnitude > m_stateMachine.MaxVelocity)
        {
            m_stateMachine.RB.velocity = m_stateMachine.RB.velocity.normalized;
            m_stateMachine.RB.velocity *= m_stateMachine.MaxVelocity;
        }
    }

    public override void OnExit()
    {
        //Debug.Log("Exit state: Free state");
    }

    public override bool CanEnter()
    {
        // Can only enter the free state only if I touch the ground
        return m_stateMachine.IsInContactWithFloor();
    }

    public override bool CanExit()
    {
        return true;
    }
}
