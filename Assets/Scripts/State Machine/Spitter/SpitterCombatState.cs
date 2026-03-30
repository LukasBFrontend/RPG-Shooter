using UnityEngine;

public class SpitterCombatState : NPCBaseState
{
    Attack _attack;
    Controller _movement;
    public SpitterCombatState(NPCStateMachine currentContext, NPCStateFactory npcStateFactory) : base(currentContext, npcStateFactory)
    {
        IsRootState = true;
    }
    public override void EnterState()
    {
        _attack = Ctx.NPC.PrimaryAttack;
        _movement = Ctx.NPC.Controller;
        Ctx.Animator.SetBool("IsAwake", true);
    }

    public override void UpdateState()
    {
        AdjustMoveSpeed();
        _attack.Attempt(Ctx.NPC, Ctx.PlayerPosition - Ctx.Position);


        CheckSwitchStates();
    }

    public override void ExitState() { }

    public override void CheckSwitchStates()
    {

    }

    public override void InitializeSubState() { }

    void AdjustMoveSpeed()
    {
        float _t = -Mathf.Pow(Mathf.Clamp(Vector2.Distance(Ctx.Player.ColliderCenter(), Ctx.NPC.ColliderCenter()) - 1f, 0f, float.MaxValue) * 2f, 2f) + 1f;
        Ctx.NPC.Controller.MovespeedMultiplier = Mathf.Lerp(1f, 0f, _t);
    }
}
