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
            m_stateMachine.RB.AddForce(-vectorOnFloor * m_stateMachine.AccelerationValue, ForceMode.Acceleration);
        }
        if (Input.GetKey(KeyCode.A))
        {
            m_stateMachine.RB.AddForce(-m_stateMachine.Camera.transform.right * m_stateMachine.AccelerationValue, ForceMode.Acceleration);
        }
        if (Input.GetKey(KeyCode.D))
        {
            m_stateMachine.RB.AddForce(m_stateMachine.Camera.transform.right * m_stateMachine.AccelerationValue, ForceMode.Acceleration);
        }
        if (m_stateMachine.RB.velocity.magnitude > m_stateMachine.MaxVelocity)
        {
            m_stateMachine.RB.velocity = m_stateMachine.RB.velocity.normalized;
            m_stateMachine.RB.velocity *= m_stateMachine.MaxVelocity;
        }

        //TODO 31 AOÛT:
        //Appliquer les déplacements relatifs à la caméra dans les 3 autres directions
        //Avoir des vitesses de déplacements maximales différentes vers les côtés et vers l'arrière
        //Lorsqu'aucun input est mis, décélérer le personnage rapidement

        Debug.Log(m_stateMachine.RB.velocity.magnitude);
    }

    public override void OnExit()
    {
    }
}
