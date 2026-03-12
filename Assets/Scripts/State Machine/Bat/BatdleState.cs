using UnityEngine;

public class BatIdleState : NPCBaseState
{
    public BatIdleState(NPCStateMachine currentContext, NPCStateFactory npcStateFactory) : base(currentContext, npcStateFactory)
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
        Debug.Log($"Bat in combat state, distance to player: {_distanceToPlayer}");
        Debug.Log($"Player position: {Ctx.PlayerPosition}, Bat position: {Ctx.Position}");
        if (_distanceToPlayer < Ctx.DetectionRange)
        {
            SwitchState(Factory.Combat());
        }
    }

    public override void InitializeSubState() { }
}
