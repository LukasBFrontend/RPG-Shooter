public class BatStateFactory : NPCStateFactory
{

    public BatStateFactory(NPCStateMachine currentContext) : base(currentContext) { }

    public override NPCBaseState Idle()
    {
        return new BatIdleState(_context, this);
    }
    public override NPCBaseState Alert()
    {
        return new BatAlertState(_context, this);
    }
    public override NPCBaseState Combat()
    {
        return new BatCombatState(_context, this);
    }
}
