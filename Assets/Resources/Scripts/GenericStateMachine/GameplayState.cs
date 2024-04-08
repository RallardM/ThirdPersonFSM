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
        return GameManagerSM.GetInstance().DesiredState == this;
    }

    public bool CanExit()
    {
        return GameManagerSM.GetInstance().DesiredState != this;
    }

    public void OnEnter()
    {
        Debug.Log("On Enter GameplayState");

        CinemachineBrain brain = CinemachineCore.Instance.GetActiveBrain(0);

        if (brain != null)
        {
            brain.ActiveVirtualCamera?.VirtualCameraGameObject.SetActive(false);
        }

        m_camera.gameObject.SetActive(true);
        GameManagerSM.GetInstance().DesiredState = null;
        GameManagerSM.GetInstance().CharacterControllerStateMachine.OnGameManagerStateChange(false);
    }

    public void OnExit()
    {
        Debug.Log("On Exit GameplayState");
    }

    public void OnFixedUpdate()
    {
        if (Input.GetMouseButtonDown(1))
        {
            // Activate the bullet time
            GameManagerSM.GetInstance().ActivateBulletTime();
        }
    }

    public void OnStart()
    {

    }

    public void OnUpdate()
    {
    }
}