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
        Vector3 vectOnFloor = Vector3.zero;

        if (!Input.anyKey)
        {
            m_stateMachine.RB.velocity = Vector3.zero;
        }
        if (Input.GetKey(KeyCode.W))
        {
            vectOnFloor = Vector3.ProjectOnPlane(m_stateMachine.Camera.transform.forward, Vector3.up);
            vectOnFloor.Normalize();
            newDirection += vectOnFloor * m_stateMachine.AccelerationValue;
        }
        if (Input.GetKey(KeyCode.S))
        {
            vectOnFloor = Vector3.ProjectOnPlane(m_stateMachine.Camera.transform.forward, Vector3.up);
            vectOnFloor.Normalize();
            newDirection -= vectOnFloor * m_stateMachine.AccelerationValue;
        }
        if (Input.GetKey(KeyCode.A))
        {
            vectOnFloor = Vector3.ProjectOnPlane(m_stateMachine.Camera.transform.right, Vector3.up);
            vectOnFloor.Normalize();
            newDirection -= vectOnFloor * m_stateMachine.AccelerationValue;
        }
        if (Input.GetKey(KeyCode.D))
        {
            vectOnFloor = Vector3.ProjectOnPlane(m_stateMachine.Camera.transform.right, Vector3.up);
            vectOnFloor.Normalize();
            newDirection += vectOnFloor * m_stateMachine.AccelerationValue;
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

        // Rotate the player's mesh
        if (newDirection != Vector3.zero)
        {
            Debug.Log("vectOnFloor : " + vectOnFloor);
            Quaternion meshRotation = Quaternion.LookRotation(vectOnFloor, Vector3.up);
            float interpolationSpeed = 4.0f;
            // Source : https://forum.unity.com/threads/what-is-the-difference-of-quaternion-slerp-and-lerp.101179/
            m_stateMachine.RB.rotation = Quaternion.Slerp(m_stateMachine.RB.rotation, meshRotation, interpolationSpeed * Time.deltaTime);
        }

        // Update the character animation
        m_stateMachine.UpdateAnimatorValues(vectOnFloor);

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
