public abstract class NPCBaseState
{
    bool _isRootState = false;
    NPCStateMachine _ctx;
    NPCStateFactory _factory;
    NPCBaseState _currentSuperState;
    NPCBaseState _currentSubState;
    protected bool IsRootState { get { return _isRootState; } set { _isRootState = value; } }
    protected NPCStateMachine Ctx { get { return _ctx; } }
    protected NPCStateFactory Factory { get { return _factory; } }

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
        _currentSubState?.UpdateStates();
    }

    protected void SwitchState(NPCBaseState newState)
    {
        ExitState();
        newState.EnterState();

        if (_isRootState)
        {
            Ctx.CurrentState = newState;
        }
        else
        {
            _currentSuperState?.SetSubState(newState);
        }
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
