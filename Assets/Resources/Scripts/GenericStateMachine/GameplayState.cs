using Cinemachine;
using UnityEngine;

public class GameplayState : IState
{
    protected Camera m_camera;

    public GameplayState(Camera camera)
    {
        m_camera = camera;
    }

    public bool CanEnter(IState currentState)
    {
        Debug.Log("CanEnter GameplayState " + (GameManagerSM.GetInstance().DesiredState == this));
        return Input.GetKeyDown(KeyCode.G) || GameManagerSM.GetInstance().DesiredState == this;
    }

    public bool CanExit()
    {
        return Input.GetKeyDown(KeyCode.G) || GameManagerSM.GetInstance().DesiredState != this;
    }

    public void OnEnter()
    {
        Debug.Log("On Enter GameplayState");
        //m_camera.enabled = true;
        CinemachineCore.Instance.GetActiveBrain(0)?.ActiveVirtualCamera.VirtualCameraGameObject.SetActive(false);
        m_camera.gameObject.SetActive(true); 
        GameManagerSM.GetInstance().DesiredState = null;
        GameManagerSM.GetInstance().CharacterControllerStateMachine.OnGameManagerStateChange(false);
    }

    public void OnExit()
    {
        Debug.Log("On Exit GameplayState");
        //m_camera.enabled = false;
    }

    public void OnFixedUpdate()
    {
    }

    public void OnStart()
    {
        //Debug.Log("GameplayState OnStart()"); // TODO: Remove after debugging
    }

    public void OnUpdate()
    {
    }
}