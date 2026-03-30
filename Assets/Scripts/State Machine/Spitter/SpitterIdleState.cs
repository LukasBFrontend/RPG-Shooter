using UnityEngine;

public class SpitterIdleState : NPCBaseState
{
    public SpitterIdleState(NPCStateMachine currentContext, NPCStateFactory npcStateFactory) : base(currentContext, npcStateFactory)
    {
        IsRootState = true;
    }
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
        float _distanceToPlayer = Vector2.Distance(Ctx.Position, Ctx.PlayerPosition);
        if (_distanceToPlayer < Ctx.DetectionRange)
        {
            SwitchState(Factory.Combat());
        }
    }

    public override void InitializeSubState() { }
}
