using UnityEngine;

public class NPCGroundedState : NPCBaseState
{
    public NPCGroundedState(NPCStateMachine currentContext, NPCStateFactory npcStateFactory) : base(currentContext, npcStateFactory)
    {
        IsRootState = true;
    }
    public override void EnterState()
    {
        InitializeSubState();
        Ctx.NPC.Controller.Move(Vector2.zero);
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }

    public override void CheckSwitchStates()
    {
        if (Ctx.IsJumpPressed)
        {
            SwitchState(Factory.Jump());
        }
    }

    public override void InitializeSubState()
    {
        if (Ctx.SeesPlayer())
        {
            SetSubState(Factory.Combat());
        }
        else if (Vector2.Distance(Ctx.NPC.ColliderCenter(), Ctx.Player.ColliderCenter()) < Ctx.DetectionRange)
        {
            SetSubState(Factory.Alert());
        }
        else
        {
            SetSubState(Factory.Idle());
        }
    }
    public override void ExitState() { }
}
