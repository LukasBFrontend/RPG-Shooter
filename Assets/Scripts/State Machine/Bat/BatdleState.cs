using UnityEngine;

public class BatIdleState : NPCBaseState
{
    public BatIdleState(NPCStateMachine currentContext, NPCStateFactory npcStateFactory) : base(currentContext, npcStateFactory) { }
    public override void EnterState()
    {
        Debug.Log("Entered idle state!");
        Ctx.NPC.Movement.Input = Vector2.zero;
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
            SwitchState(_factory.Combat());
        }
    }

    public override void InitializeSubState() { }
}
