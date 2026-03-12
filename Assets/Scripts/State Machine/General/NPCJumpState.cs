using System.Collections;
using UnityEngine;

public class NPCJumpState : NPCBaseState
{
    Attack _attack;
    bool _landing = false;
    public NPCJumpState(NPCStateMachine currentContext, NPCStateFactory npcStateFactory) : base(currentContext, npcStateFactory)
    {
        IsRootState = true;
        _attack = Ctx.NPC.PrimaryAttack;
    }
    public override void EnterState()
    {
        Ctx.BodyCollider.IsJumping = true;
        _landing = false;
        Ctx.NPC.StartCoroutine(JumpRoutine(_attack.Lifetime));
    }
    public override void UpdateState()
    {
        CheckSwitchStates();
    }
    IEnumerator JumpRoutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        _landing = true;
    }

    public override void ExitState()
    {
        Ctx.Controller.Move(Vector2.zero);
        Ctx.BodyCollider.IsJumping = false;
    }
    public override void CheckSwitchStates()
    {
        if (_landing)
        {
            SwitchState(Factory.Grounded());
        }
    }
    public override void InitializeSubState() { }
}
