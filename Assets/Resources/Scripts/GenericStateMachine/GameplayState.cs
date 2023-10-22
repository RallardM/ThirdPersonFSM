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
        if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log("GameplayState CanEnter() G is pressed");
        }

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
        Debug.Log("GameplayState OnStart()"); // TODO: Remove after debugging
    }

    public void OnUpdate()
    {
    }
}