using UnityEngine;

public class SceneTransitionState : IState
{
    public SceneTransitionState()
    {
    }

    public bool CanEnter(IState currentState)
    {
        return false;
    }

    public bool CanExit()
    {
        return false;
    }

    public void OnEnter()
    {
        Debug.Log("On Enter SceneTransitionState");
    }

    public void OnExit()
    {
        Debug.Log("On Exit SceneTransitionState");
    }

    public void OnFixedUpdate()
    {
    }

    public void OnStart()
    {
        Debug.Log("SceneTransitionState OnStart()"); // TODO: Remove after debugging
    }

    public void OnUpdate()
    {
    }
}
