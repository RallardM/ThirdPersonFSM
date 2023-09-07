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

    public virtual void OnExit()
    {
    }

    public virtual void OnFixedUpdate()
    {
    }

    public virtual void OnUpdate()
    {
    }
}
