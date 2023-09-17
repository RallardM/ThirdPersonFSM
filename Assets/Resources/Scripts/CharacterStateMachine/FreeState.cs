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

        // Calculate the camera's forward and right vectors projected on the horizontal plane
        Vector3 cameraForward = Vector3.ProjectOnPlane(m_stateMachine.Camera.transform.forward, Vector3.up).normalized;
        Vector3 cameraRight = Vector3.ProjectOnPlane(m_stateMachine.Camera.transform.right, Vector3.up).normalized;

        if (!Input.anyKey)
        {
            m_stateMachine.RB.velocity = Vector3.zero;
        }
        if (Input.GetKey(KeyCode.W))
        {
            newDirection += cameraForward * m_stateMachine.AccelerationValue;
        }
        if (Input.GetKey(KeyCode.S))
        {
            newDirection -= cameraForward * m_stateMachine.AccelerationValue;
        }
        if (Input.GetKey(KeyCode.A))
        {
            newDirection -= cameraRight * m_stateMachine.AccelerationValue;
        }
        if (Input.GetKey(KeyCode.D))
        {
            newDirection += cameraRight * m_stateMachine.AccelerationValue;
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

        // Update the character animation
        Vector3 movementValue = newDirection.normalized;
        m_stateMachine.UpdateAnimatorValues(movementValue);

        // Rotate the player's mesh
        vectOnFloor = RotatePlayerMesh(vectOnFloor, cameraRight, movementValue);

        // Apply the new direction to the rigidbody
        m_stateMachine.RB.AddForce(newDirection, ForceMode.Acceleration);
    }

    private Vector3 RotatePlayerMesh(Vector3 vectOnFloor, Vector3 cameraRight, Vector3 movementValue)
    {
        if (movementValue == Vector3.zero)
        {
            return vectOnFloor;
        }

        // Rotate the mesh on itself if pressed forward
        if (Input.GetKey(KeyCode.W))
        {
            vectOnFloor = Vector3.ProjectOnPlane(movementValue, Vector3.up).normalized;
            Quaternion meshRotation = Quaternion.LookRotation(vectOnFloor, Vector3.up);
            float interpolationSpeed = 4.0f;
            m_stateMachine.RB.rotation = Quaternion.Slerp(m_stateMachine.RB.rotation, meshRotation, interpolationSpeed * Time.deltaTime);
        }

        // Rotate the mesh in reference to the camera if pressed left
        if (Input.GetKey(KeyCode.D))
        {
            // Calculate the mesh rotation based on the camera's right vector
            vectOnFloor = Vector3.ProjectOnPlane(cameraRight, Vector3.up).normalized;
            Quaternion meshRotation = Quaternion.LookRotation(vectOnFloor, Vector3.up);
            float interpolationSpeed = 1.0f;
            m_stateMachine.RB.rotation = Quaternion.Slerp(m_stateMachine.RB.rotation, meshRotation, interpolationSpeed * Time.deltaTime);
        }

        // Rotate the mesh in reference to the camera if pressed right
        if (Input.GetKey(KeyCode.A))
        {
            // Calculate the mesh rotation based on the camera's right vector
            vectOnFloor = Vector3.ProjectOnPlane(-cameraRight, Vector3.up).normalized;
            Quaternion meshRotation = Quaternion.LookRotation(vectOnFloor, Vector3.up);
            float interpolationSpeed = 1.0f;
            m_stateMachine.RB.rotation = Quaternion.Slerp(m_stateMachine.RB.rotation, meshRotation, interpolationSpeed * Time.deltaTime);
        }

        return vectOnFloor;
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
