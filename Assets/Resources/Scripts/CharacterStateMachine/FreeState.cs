using UnityEngine;

public class FreeState : CharacterState
{
    public override void OnEnter()
    {
        //Debug.Log("Enter state: Free State");
    }

    public override void OnExit()
    {
        //Debug.Log("Exit state: Free state");
    }

    public override void OnUpdate()
    {
    }

    public override void OnFixedUpdate()
    {
        if (GameManagerSM.GetInstance().CanPlayerMove == false)
        {
            m_stateMachine.RB.velocity = Vector3.zero;
            m_stateMachine.UpdateAnimatorMovements(Vector3.zero);
            return;
        }

        Vector3 newDirection = Vector3.zero;

        // Calculate the camera's forward and right vectors projected on the horizontal plane
        Vector3 cameraForward = Vector3.ProjectOnPlane(m_stateMachine.Camera.transform.forward, Vector3.up).normalized;
        Vector3 cameraRight = Vector3.ProjectOnPlane(m_stateMachine.Camera.transform.right, Vector3.up).normalized;
        float forwardInput = 0;
        float rightInput = 0;

        if (!Input.anyKey)
        {
            m_stateMachine.RB.velocity = Vector3.zero;
        }
        if (Input.GetKey(KeyCode.W))
        {
            newDirection += cameraForward * m_stateMachine.AccelerationValue;
            forwardInput = 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            newDirection -= cameraForward * m_stateMachine.AccelerationValue;
            forwardInput = -0.5f; // Exception to reduce acceleration when moving backward so it does not stop completely
        }
        if (Input.GetKey(KeyCode.A))
        {
            newDirection -= cameraRight * m_stateMachine.AccelerationValue;
            forwardInput = -1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            newDirection += cameraRight * m_stateMachine.AccelerationValue;
            rightInput = 1;
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
        }

        if (forwardInput != 0 || rightInput != 0)
        {
            // TODO does not work properly, it slows the player in world position instead of local position
            //Debug.Log("Forward input : " + forwardInput + " Right input : " + rightInput);
            // Source : https://www.youtube.com/watch?v=mFOi6W7lohk 
            float inputAngle = Mathf.Atan2(rightInput, forwardInput);
            //Debug.Log("Input angle : " + inputAngle);
            float forwardContribution = Mathf.Cos(inputAngle) * forwardInput * m_stateMachine.AccelerationValue;
            float rightContribution = Mathf.Sin(inputAngle) * rightInput * m_stateMachine.AccelerationValue;

            newDirection += cameraForward * forwardContribution;
            newDirection += cameraRight * rightContribution;
        }
        else
        {
            m_stateMachine.RB.velocity = Vector3.zero;
        }

        // Update the character animation
        Vector3 movementValue = newDirection.normalized;
        m_stateMachine.UpdateAnimatorMovements(movementValue);

        // Rotate the player's mesh
        RotatePlayerMesh(cameraRight, movementValue);

        // Apply the new direction to the rigidbody
        m_stateMachine.RB.AddForce(newDirection, ForceMode.Acceleration);
    }

    private void RotatePlayerMesh(Vector3 cameraRight, Vector3 movementValue)
    {
        if (movementValue == Vector3.zero)
        {
            return;
        }

        // Rotate the mesh on itself if pressed forward
        if (Input.GetKey(KeyCode.W))
        {
            Vector3 vectOnFloor = Vector3.ProjectOnPlane(movementValue, Vector3.up).normalized;
            Quaternion meshRotation = Quaternion.LookRotation(vectOnFloor, Vector3.up);
            float interpolationSpeed = 4.0f;
            m_stateMachine.RB.rotation = Quaternion.Slerp(m_stateMachine.RB.rotation, meshRotation, interpolationSpeed * Time.deltaTime);
        }

        // Rotate the mesh in reference to the camera if pressed left
        if (Input.GetKey(KeyCode.D))
        {
            // Calculate the mesh rotation based on the camera's right vector
            Vector3 vectOnFloor = Vector3.ProjectOnPlane(cameraRight, Vector3.up).normalized;
            Quaternion meshRotation = Quaternion.LookRotation(vectOnFloor, Vector3.up);
            float interpolationSpeed = 1.0f;
            m_stateMachine.RB.rotation = Quaternion.Slerp(m_stateMachine.RB.rotation, meshRotation, interpolationSpeed * Time.deltaTime);
        }

        // Rotate the mesh in reference to the camera if pressed right
        if (Input.GetKey(KeyCode.A))
        {
            // Calculate the mesh rotation based on the camera's right vector
            Vector3 vectOnFloor = Vector3.ProjectOnPlane(-cameraRight, Vector3.up).normalized;
            Quaternion meshRotation = Quaternion.LookRotation(vectOnFloor, Vector3.up);
            float interpolationSpeed = 1.0f;
            m_stateMachine.RB.rotation = Quaternion.Slerp(m_stateMachine.RB.rotation, meshRotation, interpolationSpeed * Time.deltaTime);
        }
    }

    public override bool CanEnter(IState currentState)
    {
        IState attackState = currentState as AttackState;
        if (attackState != null)
        {
            if (attackState.CanExit())
            {
                //Debug.Log("Attack state can enter free state");
                return true;
            }

            return false;
        }

        IState freeState = currentState as FreeState;
        if (freeState != null)
        {
            //Debug.Log("Can enter freestate  : " + currentState.GetType().Name);
            // If not freestate check if in contact with floor
            if (m_stateMachine.IsInContactWithFloor())
            {
                Debug.Log("Can enter freestate  : " + currentState.GetType().Name);
                return true;
            }
            return false;
        }

        //Debug.Log("Try enter freestate  : " + currentState.GetType().Name);
        return m_stateMachine.IsInContactWithFloor() && m_stateMachine.IsStunned == false;
    }

    public override bool CanExit()
    {
        return true;
    }
}