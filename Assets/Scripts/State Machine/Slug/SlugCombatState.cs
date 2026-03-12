using UnityEngine;

public class SlugCombatState : NPCBaseState
{
    Attack _attack;
    Controller _movement;
    public SlugCombatState(NPCStateMachine currentContext, NPCStateFactory npcStateFactory) : base(currentContext, npcStateFactory)
    {
        IsRootState = true;
    }
    public override void EnterState()
    {
        _attack = Ctx.NPC.PrimaryAttack;
        _movement = Ctx.NPC.Controller;
    }

    public override void UpdateState()
    {
        SetTransformRotation();

        if (_attack.IsWindingUp)
        {
            return;
        }
        AdjustMoveSpeed();
        _movement.FollowCharacter(Ctx.Player, Ctx.ClusterSignal);
        _attack.Attempt(Ctx.NPC, _attack.CharactersInRange.ToArray());

        CheckSwitchStates();
    }

    public override void ExitState() { }

    public override void CheckSwitchStates()
    {
        if (!Ctx.SeesPlayer())
        {
            SwitchState(Factory.Alert());
        }
    }

    public override void InitializeSubState() { }

    void AdjustMoveSpeed()
    {
        float _t = -Mathf.Pow(Mathf.Clamp(Vector2.Distance(Ctx.Player.ColliderCenter(), Ctx.NPC.ColliderCenter()) - 1f, 0f, float.MaxValue) * 2f, 2f) + 1f;
        Ctx.NPC.Controller.MovespeedMultiplier = Mathf.Lerp(1f, 0f, _t);
    }

    void SetTransformRotation()
    {
        float _targetAngle = Mathf.Atan2(
            Ctx.NPC.Rigidbody.linearVelocity.y,
            Ctx.NPC.Rigidbody.linearVelocity.x
        ) * Mathf.Rad2Deg + 90f;

        float _currentAngle = Ctx.transform.eulerAngles.z;
        float _angleDifference = Mathf.Abs(Mathf.DeltaAngle(_currentAngle, _targetAngle));

        float _scaledFactor = Mathf.Lerp(.5f, 1f, _angleDifference / 180);

        float _baseTurnSpeed = 270f;
        float _turnSpeed = _baseTurnSpeed * _scaledFactor;

        float _newAngle = Mathf.MoveTowardsAngle(
            _currentAngle,
            _targetAngle,
            _turnSpeed * Time.deltaTime
        );

        Ctx.transform.rotation = Quaternion.Euler(0f, 0f, _newAngle);
    }
}
