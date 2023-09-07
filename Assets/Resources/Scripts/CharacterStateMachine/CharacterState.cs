using System.Diagnostics;

public class CharacterState : IState
{
    protected CharacterControllerStateMachine m_stateMachine;

    public void OnStart(CharacterControllerStateMachine stateMachine)
    {
        m_stateMachine = stateMachine;
    }

    public virtual void OnEnter()
    {
    }

    public virtual void OnFixedUpdate()
    {
    }

    public virtual void OnUpdate()
    {
    }

    public virtual void OnExit()
    {
    }

    public virtual bool CanEnter()
    {
        return false;
    }

    public virtual bool CanExit()
    {
        return false;
    }
}
