public abstract class CharacterState : IState
{
    protected CharacterControllerStateMachine m_stateMachine;

    public void OnStart()
    {
        throw new System.NotImplementedException();
    }

    public virtual void OnStart(CharacterControllerStateMachine stateMachineRef)
    {
        //TODO: Refactor in progress
        //Do not forget to call and send the state machine
        //To characterStates
        m_stateMachine = stateMachineRef;
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

    public virtual bool CanEnter(IState currentState)
    {
        throw new System.NotImplementedException();
    }

    public virtual bool CanExit()
    {
        throw new System.NotImplementedException();
    }
}