using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlugAlertState : NPCBaseState
{
    const float MOVE_THRESHOLD = .1f;
    const float REFRESH_RATE = 3f;
    List<Node> _path;
    bool _refreshPath = false;
    bool _suspendPathing = false;
    Vector2 _storedTargetPosition;
    public SlugAlertState(NPCStateMachine currentContext, NPCStateFactory npcStateFactory) : base(currentContext, npcStateFactory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        Ctx.NPC.StartCoroutine(SuspendPathingRoutine(.5f));
    }

    public override void UpdateState()
    {
        SetTransformRotation();

        if (_suspendPathing)
        {
            return;
        }

        if (_path.Count == 0 || _refreshPath && Vector2.Distance(_path[0].transform.position, Ctx.Position) < .1f)
        {
            Ctx.NPC.StartCoroutine(RefreshPathRoutine());
        }

        Ctx.Controller.FollowPath(_path);

        CheckSwitchStates();
    }

    public override void ExitState() { }

    public override void CheckSwitchStates()
    {
        if (Ctx.SeesPlayer())
        {
            SwitchState(Factory.Combat());
        }
    }

    public override void InitializeSubState() { }

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
    IEnumerator SuspendPathingRoutine(float delay)
    {
        _suspendPathing = true;
        yield return new WaitForSeconds(delay);
        _suspendPathing = false;

        Ctx.NPC.Node = NodeManager.Instance.ClosestNode(Ctx.Position + Ctx.NPC.Controller.Input);
        Ctx.NPC.StartCoroutine(RefreshPathRoutine());
    }

    IEnumerator RefreshPathRoutine()
    {
        Node _playerNode = NodeManager.Instance.ClosestNode(Ctx.PlayerPosition);
        Ctx.NPC.Node = NodeManager.Instance.ClosestNode(Ctx.Position);

        _path = Paths.AStar(
            Ctx.NPC.Node,
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
