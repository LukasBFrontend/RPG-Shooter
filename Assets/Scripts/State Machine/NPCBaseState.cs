public abstract class NPCBaseState
{
    protected bool _isRootState = false;
    protected NPCStateMachine _ctx;
    protected NPCStateFactory _factory;
    NPCBaseState _currentSuperState;
    NPCBaseState _currentSubState;
    protected bool IsRootState { set { _isRootState = value; } }
    protected NPCStateMachine Ctx { get { return _ctx; } }
    protected NPCStateMachine Factory { get { return _ctx; } }

    public NPCBaseState(NPCStateMachine currentContext, NPCStateFactory npcStateFactory)
    {
        _ctx = currentContext;
        _factory = npcStateFactory;
    }
    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
    public abstract void CheckSwitchStates();
    public abstract void InitializeSubState();

    public void UpdateStates()
    {
        UpdateState();
        _currentSubState?.UpdateState();
    }

    protected void SwitchState(NPCBaseState newState)
    {
        ExitState();
        newState.EnterState();

        if (_isRootState)
        {
            _ctx.CurrentState = newState;
        }
        else if (_currentSuperState != null)
        {
            _currentSuperState.SetSubState(newState);
        }

        _ctx.CurrentState = newState;
    }

    protected void SetSuperState(NPCBaseState newSuperState)
    {
        _currentSuperState = newSuperState;
    }

    protected void SetSubState(NPCBaseState newSubState)
    {
        _currentSubState = newSubState;
        newSubState.SetSuperState(this);
    }
}
