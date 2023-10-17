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
        return Input.GetKeyDown(KeyCode.G);
    }

    public bool CanExit()
    {
        return Input.GetKeyDown(KeyCode.G);
    }

    public void OnEnter()
    {
        Debug.Log("On Enter GameplayState");
        m_camera.enabled = true;
    }

    public void OnExit()
    {
        Debug.Log("On Exit GameplayState");
        m_camera.enabled = false;
    }

    public void OnFixedUpdate()
    {
    }

    public void OnStart()
    {
    }

    public void OnUpdate()
    {
    }
}