using UnityEngine;

public class SlugStateFactory : NPCStateFactory
{

    public SlugStateFactory(NPCStateMachine currentContext) : base(currentContext) { }

    public override NPCBaseState Idle()
    {
        return new SlugIdleState(_context, this);
    }
    public override NPCBaseState Alert()
    {
        return new SlugAlertState(_context, this);
    }
    public override NPCBaseState Combat()
    {
        return new SlugCombatState(_context, this);
    }


}
