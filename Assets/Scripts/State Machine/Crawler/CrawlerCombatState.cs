using System.Collections;
using UnityEngine;

public class CrawlerCombatState : NPCBaseState
{
    const float PATH_CHECK_DISTANCE = 2f;
    private static WaitForSeconds _waitForSeconds0_1 = new(0.1f);
    Attack _attack;
    public CrawlerCombatState(NPCStateMachine currentContext, NPCStateFactory npcStateFactory) : base(currentContext, npcStateFactory)
    {
        _attack = Ctx.NPC.PrimaryAttack;
    }
    public override void EnterState()
    {
        Ctx.Controller.Move(Vector2.zero);
    }

    public override void UpdateState()
    {
        if (_attack.OnCooldown)
        {
            return;
        }

        _attack.Attempt(Ctx.NPC, RandomClearDirection());
        Ctx.NPC.StartCoroutine(PressJumpRoutine(_attack.WindUp));
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

    Vector2 RandomClearDirection()
    {
        const float MAX_ITERATIONS = 20;
        Vector2 _playerDir = Ctx.PlayerPosition - Ctx.Position;
        float _playerAngle = Mathf.Atan2(_playerDir.y, _playerDir.x);

        for (int i = 0; i < MAX_ITERATIONS; i++)
        {
            float _random = Random.Range(-Mathf.PI / 2, Mathf.PI / 2);
            float _angle = _playerAngle + _random;
            Vector2 _dir = new(Mathf.Cos(_angle), Mathf.Sin(_angle));
            if (Ctx.IsPathClear(Ctx.NPC.ColliderCenter(), _dir, PATH_CHECK_DISTANCE))
            {
                return _dir;
            }
        }
        return Vector2.zero;
    }

    IEnumerator PressJumpRoutine(float windUp)
    {
        yield return new WaitForSeconds(windUp);
        Ctx.IsJumpPressed = true;
        yield return _waitForSeconds0_1;
        Ctx.IsJumpPressed = false;
    }
}
