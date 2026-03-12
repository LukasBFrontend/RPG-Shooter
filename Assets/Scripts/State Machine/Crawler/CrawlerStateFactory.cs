using UnityEngine;

public class CrawlerStateFactory : NPCStateFactory
{

    public CrawlerStateFactory(NPCStateMachine currentContext) : base(currentContext) { }

    public override NPCBaseState Idle()
    {
        return new CrawlerIdleState(_context, this);
    }
    public override NPCBaseState Alert()
    {
        return new CrawlerAlertState(_context, this);
    }
    public override NPCBaseState Combat()
    {
        return new CrawlerCombatState(_context, this);
    }

    public override NPCBaseState Jump()
    {
        return new NPCJumpState(_context, this);
    }

    public override NPCBaseState Grounded()
    {
        return new NPCGroundedState(_context, this);
    }
}
