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
            Vector3 vectOnFloorDollyDir = Vector3.ProjectOnPlane(m_stateMachine.Camera.transform.forward, Vector3.up);
            vectOnFloorDollyDir.Normalize();
            newDirection -= vectOnFloorDollyDir * m_stateMachine.AccelerationValue;
        }
        if (Input.GetKey(KeyCode.A))
        {
            Vector3 vectOnFloorTruckDir = Vector3.ProjectOnPlane(m_stateMachine.Camera.transform.right, Vector3.up);
            vectOnFloorTruckDir.Normalize();
            newDirection -= vectOnFloorTruckDir * m_stateMachine.AccelerationValue;
        }
        if (Input.GetKey(KeyCode.D))
        {
            Vector3 vectOnFloorTruckDir = Vector3.ProjectOnPlane(m_stateMachine.Camera.transform.right, Vector3.up);
            vectOnFloorTruckDir.Normalize();
            newDirection += vectOnFloorTruckDir * m_stateMachine.AccelerationValue;
        }

        // Rotate the player's mesh toward the new input direction
        if (newDirection != Vector3.zero)
        {
            Quaternion meshRotation = Quaternion.LookRotation(newDirection, Vector3.up);
            float interpolationSpeed = 2.0f;
            // Source : https://forum.unity.com/threads/what-is-the-difference-of-quaternion-slerp-and-lerp.101179/
            m_stateMachine.PlayerTransform.rotation = Quaternion.Slerp(m_stateMachine.PlayerTransform.rotation, meshRotation, interpolationSpeed * Time.deltaTime);
        }

        /*
         *  Par exemple, si vous allez à un angle nord-nord-ouest (3/4 du déplacement 	
         *  vers l'avant, 1/4 vers la gauche), et que votre vitesse maximale de 	
         *  déplacement avant est 20 et vers les côtés 5, votre vitesse maximale calculée 	
         *  à ce moment devrait être de ((3/4) * 20 + (1/4) * 5) == 15 + 1.25 == 16.25
         */

        // Limit the velocity of the player
        if (m_stateMachine.RB.velocity.magnitude > m_stateMachine.MaxVelocity)
        {
            m_stateMachine.RB.velocity = m_stateMachine.RB.velocity.normalized;
            m_stateMachine.RB.velocity *= m_stateMachine.MaxVelocity;
            //Vector3 currentDirectionNorm = m_stateMachine.RB.transform.forward.normalized;
            //Vector3 newDirectionNorm = newDirection.normalized;
            //float dotProduct = Vector3.Dot(currentDirectionNorm, newDirectionNorm);
            //Debug.Log("Dot product : " + dotProduct);
            //m_stateMachine.RB.velocity = (dotProduct;

        }

        m_stateMachine.UpdateAnimatorValues(newDirection);

        // Apply the new direction to the rigidbody
        m_stateMachine.RB.AddForce(newDirection, ForceMode.Acceleration);
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
