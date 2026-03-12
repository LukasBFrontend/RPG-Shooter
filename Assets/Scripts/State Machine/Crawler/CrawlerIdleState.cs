using UnityEngine;

public class CrawlerIdleState : NPCBaseState
{
    public CrawlerIdleState(NPCStateMachine currentContext, NPCStateFactory npcStateFactory) : base(currentContext, npcStateFactory) { }
    public override void EnterState()
    {
        Ctx.NPC.Controller.Move(Vector2.zero);
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }

    public override void ExitState() { }

    public override void CheckSwitchStates()
    {
        if (Vector2.Distance(Ctx.NPC.transform.position, Ctx.Player.transform.position) < Ctx.DetectionRange)
        {
            SwitchState(Factory.Alert());
        }
    }

    public override void InitializeSubState() { }
}
