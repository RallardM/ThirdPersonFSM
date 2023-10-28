using Cinemachine;
using UnityEngine;

public class CinematicState : IState
{
    protected Camera m_camera;

    public CinematicState(Camera camera)
    {
        m_camera = camera;
    }

    public bool CanEnter(IState currentState)
    {
        //Debug.Log("CanEnter CinematicState " + (GameManagerSM.GetInstance().DesiredState == this));
        return Input.GetKeyDown(KeyCode.G) || GameManagerSM.GetInstance().DesiredState == this;
    }

    public bool CanExit()
    {
        //Debug.Log("CanExit CinematicState " + Input.GetKeyDown(KeyCode.G));
        return Input.GetKeyDown(KeyCode.G) || GameManagerSM.GetInstance().DesiredState != this;
    }

    public void OnEnter()
    {
        Debug.Log("On Enter CinematicState");

        CinemachineBrain brain = CinemachineCore.Instance.GetActiveBrain(0);
        if (brain != null)
        {
            brain.ActiveVirtualCamera?.VirtualCameraGameObject.SetActive(false);
        }

        m_camera.gameObject.SetActive(true);
        GameManagerSM.GetInstance().DesiredState = null;
        GameManagerSM.GetInstance().CharacterControllerStateMachine.OnGameManagerStateChange(true);
    }

    public void OnExit()
    {
        Debug.Log("On Exit CinematicState");
        //m_camera.enabled = false;
    }

    public void OnFixedUpdate()
    {
    }

    public void OnStart()
    {
        //Debug.Log("CinematicState OnStart()"); // TODO: Remove after debugging
    }

    public void OnUpdate()
    {
    }
}