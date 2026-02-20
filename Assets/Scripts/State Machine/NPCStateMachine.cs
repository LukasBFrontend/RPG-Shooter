
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum StateFactoryType
{
    Slug,
    Bat,
}
public class NPCStateMachine : MonoBehaviour
{
    [SerializeField] StateFactoryType stateFactoryType;
    [SerializeField] NPC npc;
    [SerializeField] bool debugRaycast = false;
    NPCBaseState _currentState;
    NPCStateFactory _states;
    Movement _movement;
    Player _player;
    List<Node> _nodeGrid;
    Node _currentNode;
    bool _seesPlayer = false;
    Vector2 _clusterSignal = new();
    public Animator Animator { get { return npc.Animator; } }
    public NPCBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public NPC NPC { get { return npc; } }
    public Player Player { get { return _player; } }
    public Vector2 PlayerPosition { get { return _player.ColliderCenter(); } }
    public Vector2 Position { get { return NPC.ColliderCenter(); } }
    public List<Node> NodeGrid { get { return _nodeGrid; } }
    public Node CurrentNode { get { return _currentNode; } set { _currentNode = value; } }
    public float DetectionRange { get { return npc.DetectionRange; } }
    public Vector2 ClusterSignal { get { return _clusterSignal; } set { _clusterSignal = value; } }

    void Awake()
    {
        Cache();
        _currentState = _states.Idle();
        _currentState.EnterState();
    }
    void Update()
    {
        _currentState.UpdateStates();
    }

    void Cache()
    {
        _states = GetNPCStateFactory(stateFactoryType);
        _nodeGrid = NodeManager.Instance.GetNodes();
        _player = GameState.Player;
        _movement = npc.Movement;
    }

    NPCStateFactory GetNPCStateFactory(StateFactoryType stateFactoryType)
    {
        switch (stateFactoryType)
        {
            case StateFactoryType.Slug:
                return new SlugStateFactory(this);
            case StateFactoryType.Bat:
                return new BatStateFactory(this);
            default:
                return new SlugStateFactory(this);
        }
    }

    public bool SeesPlayer()
    {
        Vector2 _origin =
            _seesPlayer ?
            transform.position :
            NodeManager.Instance.ClosestNode(transform.position).transform.position
        ;
        Vector2 _dir = Utils.PlayerToTransformNode(transform).normalized;
        Vector2 _perp = new(-_dir.y, _dir.x);

        float _extents = Mathf.Max(npc.Collider.bounds.extents.x, npc.Collider.bounds.extents.y);
        float _side = _extents;
        float _diagonal = Mathf.Sqrt(2 * Mathf.Pow(_extents, 2));

        float _axisAlign = Mathf.Abs(_dir.x * _dir.y) * 4f;
        _axisAlign = Mathf.Clamp01(_axisAlign);

        float _offsetDist = Mathf.Lerp(_side, _diagonal, _axisAlign);
        Vector2 _lateralOffset = _perp * _offsetDist;

        float _dist = Utils.PlayerToTransformNode(transform).magnitude;
        if (_dist <= 0)
        {
            _dist = 0.1f;
        }

        Physics2D.queriesHitTriggers = true;
        RaycastHit2D[] _hitsLeft = Physics2D.RaycastAll(_origin + _lateralOffset, _dir, _dist);
        RaycastHit2D[] _hitsRight = Physics2D.RaycastAll(_origin - _lateralOffset, _dir, _dist);
        Physics2D.queriesHitTriggers = false;
        RaycastHit2D[] _hitsDirect = Physics2D.RaycastAll(_origin, _dir, _dist);

        int _obstacleCount = ObstaclesHit(_hitsLeft) + ObstaclesHit(_hitsRight);

        if (debugRaycast)
        {
            DrawRaycasts(_origin, _dir, _dist, _lateralOffset, _hitsLeft, _hitsDirect, _hitsRight);
        }

        return _seesPlayer = _obstacleCount == 0 && IsPlayerHit(_hitsDirect);

    }

    public void FollowCharacter(Character character)
    {
        Vector2 _direction = (character.ColliderCenter() - npc.ColliderCenter()).normalized + _clusterSignal;
        npc.SetFacing(_direction);
        _movement.Input = _direction;
    }

    public void FollowPath(List<Node> path)
    {
        int x = 0;
        Vector3 _targetPos = new(path[x].transform.position.x, path[x].transform.position.y, 0);
        Vector2 _direction = (_targetPos - transform.position).normalized;
        npc.SetFacing(_direction);

        _movement.Input = _direction;

        if (Vector2.Distance(transform.position, path[x].transform.position) < 0.1f)
        {
            _currentNode = path[x];
            path.RemoveAt(x);
        }
    }

    int ObstaclesHit(RaycastHit2D[] hits)
    {
        int _obstacleCount = 0;
        foreach (RaycastHit2D hit in hits)
        {
            if (npc.RaycastIgnore.Contains(hit.collider))
            {
                continue;
            }
            else if (!Utils.PlayerTags.Contains(hit.collider.tag))
            {
                _obstacleCount++;
            }
        }

        return _obstacleCount;
    }

    bool IsPlayerHit(RaycastHit2D[] hits)
    {
        foreach (RaycastHit2D hit in hits)
        {
            if (npc.RaycastIgnore.Contains(hit.collider))
            {
                continue;
            }
            else if (Utils.PlayerTags.Contains(hit.collider.tag))
            {
                return true;
            }
        }

        return false;
    }

    void DrawRaycasts(Vector2 origin, Vector2 direction, float distance, Vector2 lateralOffset, RaycastHit2D[] hitsLeft, RaycastHit2D[] hitsMiddle, RaycastHit2D[] hitsRight)
    {
        Vector2 _endLeft = hitsLeft.Length > 0 ? hitsLeft.Last().point : (origin + lateralOffset + direction * distance);
        Vector2 _endRight = hitsRight.Length > 0 ? hitsRight.Last().point : (origin - lateralOffset + direction * distance);
        Vector2 _endMiddle = hitsMiddle.Length > 0 ? hitsMiddle.Last().point : (origin + direction * distance);

        Debug.DrawLine(origin + lateralOffset, _endLeft, Color.red);
        Debug.DrawLine(origin - lateralOffset, _endRight, Color.red);
        Debug.DrawLine(origin, _endMiddle, Color.red);

        Debug.DrawLine(origin, origin + lateralOffset, Color.yellow);
        Debug.DrawLine(origin, origin - lateralOffset, Color.yellow);
    }
}
