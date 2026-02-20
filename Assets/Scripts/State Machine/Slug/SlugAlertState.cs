using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlugAlertState : NPCBaseState
{
    const float MOVE_THRESHOLD = .1f;
    const float REFRESH_RATE = 3f;
    List<Node> _path;
    bool _refreshPath = false;
    Vector2 _storedTargetPosition;
    public SlugAlertState(NPCStateMachine currentContext, NPCStateFactory npcStateFactory) : base(currentContext, npcStateFactory) { }

    public override void EnterState()
    {
        Debug.Log("Entered alert state!");
        Ctx.CurrentNode = NodeManager.Instance.ClosestNode(Ctx.Position + Ctx.NPC.Movement.Input);
        Ctx.NPC.StartCoroutine(RefreshPathRoutine());
    }

    public override void UpdateState()
    {
        if (_path.Count == 0 || _refreshPath && Vector2.Distance(_path[0].transform.position, Ctx.Position) < .1f)
        {
            Ctx.NPC.StartCoroutine(RefreshPathRoutine());
        }

        Ctx.FollowPath(_path);

        CheckSwitchStates();
    }

    public override void ExitState() { }

    public override void CheckSwitchStates()
    {
        if (Ctx.SeesPlayer())
        {
            Debug.Log("Player spotted!");
            SwitchState(_factory.Combat());
        }
    }

    public override void InitializeSubState() { }

    IEnumerator RefreshPathRoutine()
    {
        Node _playerNode = NodeManager.Instance.ClosestNode(Ctx.PlayerPosition);
        Ctx.CurrentNode = NodeManager.Instance.ClosestNode(Ctx.Position);

        _path = Paths.AStar(
            Ctx.CurrentNode,
            _playerNode,
            Ctx.NodeGrid,
            MoveBehavior.Stable
        );

        _refreshPath = false;
        _storedTargetPosition = Ctx.PlayerPosition;
        yield return new WaitForSeconds(1f / REFRESH_RATE);

        float _distance = Vector2.Distance(_storedTargetPosition, Ctx.PlayerPosition);
        _refreshPath = true;
    }
}
