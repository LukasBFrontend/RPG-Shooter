public class SpitterStateFactory : NPCStateFactory
{

    public SpitterStateFactory(NPCStateMachine currentContext) : base(currentContext) { }

    public override NPCBaseState Idle()
    {
        return new SpitterIdleState(_context, this);
    }

    public override NPCBaseState Combat()
    {
        return new SpitterCombatState(_context, this);
    }

    public override NPCBaseState Alert()
    {
        return null;
    }

    public override NPCBaseState Jump()
    {
        return null;
    }

    public override NPCBaseState Grounded()
    {
        return null;
    }
}
