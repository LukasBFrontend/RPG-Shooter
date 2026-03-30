
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum StateFactoryType
{
    SlugStateFactory,
    CrawlerStateFactory,
    BatStateFactory,
    SpitterStateFactory,
}

[RequireComponent(typeof(NPC))]
public class NPCStateMachine : MonoBehaviour
{
    [SerializeField] StateFactoryType stateFactory;
    [SerializeField] BodyCollider bodyCollider;
    [SerializeField] bool debugRaycast = false;
    NPC _npc;
    NPCBaseState _currentState;
    NPCStateFactory _states;
    Controller _controller;
    Player _player;
    List<Node> _nodeGrid;
    bool _seesPlayer = false;
    Vector2 _clusterSignal = new();
    public Animator Animator { get { return _npc.Animator; } }
    public NPCBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public NPC NPC { get { return _npc; } }
    public Player Player { get { return _player; } }
    public Controller Controller { get { return _controller; } }
    public Vector2 Position { get { return NPC.ColliderCenter(); } }
    public Vector2 PlayerPosition { get { return _player.ColliderCenter(); } }
    public BodyCollider BodyCollider { get { return bodyCollider; } }
    public List<Node> NodeGrid { get { return _nodeGrid; } }
    public Node CurrentNode { get { return _npc.Controller.CurrentNode; } }
    public float DetectionRange { get { return _npc.DetectionRange; } }
    public Vector2 ClusterSignal { get { return _clusterSignal; } set { _clusterSignal = value; } }
    public bool IsJumpPressed { get; set; }

    void Awake()
    {
        Cache();
        _currentState = _states.Grounded() ?? _states.Idle();
        _currentState.EnterState();
    }
    void Update()
    {
        _currentState.UpdateStates();
    }

    void Cache()
    {
        _npc = GetComponent<NPC>();
        _states = GetNPCStateFactory(stateFactory);
        _nodeGrid = NodeManager.Instance.GetNodes();
        _player = GameState.Player;
        _controller = _npc.Controller;
    }

    NPCStateFactory GetNPCStateFactory(StateFactoryType stateFactoryType)
    {
        return stateFactoryType switch
        {
            StateFactoryType.SlugStateFactory => new SlugStateFactory(this),
            StateFactoryType.BatStateFactory => new BatStateFactory(this),
            StateFactoryType.CrawlerStateFactory => new CrawlerStateFactory(this),
            StateFactoryType.SpitterStateFactory => new SpitterStateFactory(this),
            _ => new SlugStateFactory(this),
        };
    }

    public bool IsPathClear(Vector2 origin, Vector2 direction, float distance)
    {
        Vector2 _dir = direction.normalized;
        Vector2 _perp = new(-_dir.y, _dir.x);

        float _extents = Mathf.Max(_npc.Collider.bounds.extents.x, _npc.Collider.bounds.extents.y);
        float _side = _extents;
        float _diagonal = Mathf.Sqrt(2 * Mathf.Pow(_extents, 2));

        float _axisAlign = Mathf.Abs(_dir.x * _dir.y) * 4f;
        _axisAlign = Mathf.Clamp01(_axisAlign);

        float _offsetDist = Mathf.Lerp(_side, _diagonal, _axisAlign);
        Vector2 _lateralOffset = _perp * _offsetDist;

        if (distance <= 0)
        {
            distance = 0.1f;
        }

        Physics2D.queriesHitTriggers = true;

        RaycastHit2D[] _hitsMiddle = Physics2D.RaycastAll(origin, _dir, distance);
        RaycastHit2D? _playerHit = IsPlayerHit(_hitsMiddle);
        if (_playerHit == null)
        {
            Physics2D.queriesHitTriggers = false;
            return false;
        }
        float _distanceToPlayerHit = Vector2.Distance(origin, _playerHit.Value.point);
        RaycastHit2D[] _hitsLeft = Physics2D.RaycastAll(origin + _lateralOffset, _dir, _distanceToPlayerHit - .5f);
        RaycastHit2D[] _hitsRight = Physics2D.RaycastAll(origin - _lateralOffset, _dir, _distanceToPlayerHit - .5f);
        Physics2D.queriesHitTriggers = false;



        float _obstructions = ObstaclesHit(_hitsLeft) + ObstaclesHit(_hitsRight);

        if (debugRaycast)
        {
            DrawRaycasts(origin, _dir, distance, _lateralOffset, _hitsLeft, _hitsMiddle, _hitsRight);
        }

        return _obstructions == 0;
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

        float _extents = Mathf.Max(_npc.Collider.bounds.extents.x, _npc.Collider.bounds.extents.y);
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

        RaycastHit2D[] _hitsMiddle = Physics2D.RaycastAll(_origin, _dir, _dist);
        RaycastHit2D? _playerHit = IsPlayerHit(_hitsMiddle);
        if (_playerHit == null)
        {
            Physics2D.queriesHitTriggers = false;
            return false;
        }
        float _distanceToPlayerHit = Vector2.Distance(_origin, _playerHit.Value.point);
        RaycastHit2D[] _hitsLeft = Physics2D.RaycastAll(_origin + _lateralOffset, _dir, _distanceToPlayerHit - .5f);
        RaycastHit2D[] _hitsRight = Physics2D.RaycastAll(_origin - _lateralOffset, _dir, _distanceToPlayerHit - .5f);
        Physics2D.queriesHitTriggers = false;

        float _obstructions = ObstaclesHit(_hitsLeft) + ObstaclesHit(_hitsRight);

        if (debugRaycast)
        {
            DrawRaycasts(_origin, _dir, _dist, _lateralOffset, _hitsLeft, _hitsMiddle, _hitsRight);
        }

        return _obstructions == 0;
    }



    int ObstaclesHit(RaycastHit2D[] hits)
    {
        int _hits = 0;
        foreach (RaycastHit2D hit in hits)
        {
            if (_npc.RaycastIgnore.Contains(hit.collider))
            {
                continue;
            }
            else if (!Utils.PlayerTags.Contains(hit.collider.tag))
            {
                _hits++;
            }
        }

        return _hits;
    }

    RaycastHit2D? IsPlayerHit(RaycastHit2D[] hits)
    {
        foreach (RaycastHit2D hit in hits)
        {
            if (_npc.RaycastIgnore.Contains(hit.collider))
            {
                continue;
            }
            else if (Utils.PlayerTags.Contains(hit.collider.tag))
            {
                return hit;
            }
        }

        return null;
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
