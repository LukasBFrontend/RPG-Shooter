using UnityEngine;

public class BatCombatState : NPCBaseState
{
    public BatCombatState(NPCStateMachine currentContext, NPCStateFactory npcStateFactory) : base(currentContext, npcStateFactory) { }
    public override void EnterState()
    {
        Debug.Log("Entered combat state!");
        Ctx.Animator.SetBool("IsAwake", true);
    }

    public override void UpdateState()
    {
        Ctx.FollowCharacter(Ctx.Player);
        AdjustMoveSpeed();
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
        Ctx.NPC.Movement.MovespeedMultiplier = Mathf.Lerp(1f, 0f, _t);
    }
}
