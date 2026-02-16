using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(NPC))]
[RequireComponent(typeof(Movement))]
public class NPC_Controller : MonoBehaviour
{
    [SerializeField] Collider2D col;
    [SerializeField] Collider2D[] raycastIgnore;
    [SerializeField] bool debugRaycast = false;
    [HideInInspector] public Node CurrentNode;
    [HideInInspector] public List<Node> Path = new();
    bool _isVisionChange, _seesPlayer, _pathQued = false;
    Vector2 _lastTrackedPos;
    float _pathingTimer = 0f;
    NPC _npc;
    Movement _movement;
    const float MOVE_THRESHOLD = 2f;
    const float REFRESH_RATE = 3f;

    void Start()
    {
        Cache();
        SetNode(new(0, 0));
    }

    void Update()
    {
        _pathingTimer -= Time.deltaTime;

        RayCastPlayer();

        if (Path == null || Path.Count == 0 || (!_seesPlayer && _isVisionChange))
        {
            CreatePath(GameState.Player.State.CurrentNode());
        }

        if (_pathingTimer <= 0f)
        {
            float _playerMoveDist = Vector2.Distance(_lastTrackedPos, Player.Config.ColliderCenter());

            if (!_seesPlayer && _playerMoveDist > MOVE_THRESHOLD)
            {
                _pathingTimer = 1 / REFRESH_RATE;
                _pathQued = true;
            }
        }

        if (_seesPlayer)
        {
            Vector2 _offset = Utils.PlayerToTransform(transform).normalized;
            SetNode(_offset);
            FollowDirect();
        }
        else
        {
            FollowPath();
        }

        AdjustMoveSpeed();
    }

    void Cache()
    {
        _pathingTimer = 1 / REFRESH_RATE;
        _npc = GetComponent<NPC>();
        _movement = GetComponent<Movement>();
    }

    void AdjustMoveSpeed()
    {
        float _t = -Mathf.Pow(Mathf.Clamp(Utils.PlayerToTransform(transform).magnitude - 1f, 0f, float.MaxValue) * 2f, 2f) + 1f;
        _movement.MovespeedMultiplier = Mathf.Lerp(1f, 0f, _t);
    }

    void SetNode(Vector2 offset)
    {
        CurrentNode = NodeManager.Instance.ClosestNode((Vector2)transform.position + offset);
    }

    public void FollowDirect()
    {
        Vector2 _direction = Utils.PlayerToTransform(transform).normalized;
        _npc.SetFacing(_direction);
        _movement.Input = _direction;

        if (_pathQued)
        {
            CreatePath(GameState.Player.State.CurrentNode());
            _pathQued = false;
        }
    }

    public void FollowPath()
    {
        if (Path == null || Path.Count == 0)
        {
            return;
        }

        int x = 0;

        Vector3 _targetPos = new(Path[x].transform.position.x, Path[x].transform.position.y, 0);
        Vector2 _direction = (_targetPos - transform.position).normalized;
        _npc.SetFacing(_direction);

        _movement.Input = _direction;

        if (Vector2.Distance(transform.position, Path[x].transform.position) < 0.1f)
        {
            CurrentNode = Path[x];
            Path.RemoveAt(x);

            if (_pathQued)
            {
                CreatePath(GameState.Player.State.CurrentNode());
                _pathQued = false;
            }
        }
    }

    void RayCastPlayer()
    {
        Vector2 _origin =
            _seesPlayer ?
            transform.position :
            NodeManager.Instance.ClosestNode(transform.position).transform.position
        ;
        Vector2 _dir = Utils.PlayerToClosestNode(transform).normalized;
        Vector2 _perp = new(-_dir.y, _dir.x);

        float _extents = Mathf.Max(col.bounds.extents.x, col.bounds.extents.y);
        float _side = _extents;
        float _diagonal = Mathf.Sqrt(2 * Mathf.Pow(_extents, 2));

        float _axisAlign = Mathf.Abs(_dir.x * _dir.y) * 4f;
        _axisAlign = Mathf.Clamp01(_axisAlign);

        float _offsetDist = Mathf.Lerp(_side, _diagonal, _axisAlign);
        Vector2 _lateralOffset = _perp * _offsetDist;

        float _dist = Utils.PlayerToClosestNode(transform).magnitude;
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

        _isVisionChange = !(_seesPlayer == (_obstacleCount == 0));
        _seesPlayer = _obstacleCount == 0 && IsPlayerHit(_hitsDirect);

        if (_isVisionChange)
        {
            _pathingTimer = 1 / REFRESH_RATE;
            _pathQued = true;
        }

        if (debugRaycast)
        {
            DrawRaycasts(_origin, _dir, _dist, _lateralOffset, _hitsLeft, _hitsDirect, _hitsRight);
        }
    }

    bool IsPlayerHit(RaycastHit2D[] hits)
    {
        foreach (RaycastHit2D hit in hits)
        {
            if (raycastIgnore.Contains(hit.collider))
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

    int ObstaclesHit(RaycastHit2D[] hits)
    {
        int _obstacleCount = 0;
        foreach (RaycastHit2D hit in hits)
        {
            if (raycastIgnore.Contains(hit.collider))
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

    public void CreatePath(Node targetNode)
    {
        if (Path == null || CurrentNode == null)
        {
            Path = new();
            return;
        }
        Path.Clear();

        _lastTrackedPos = Player.Config.ColliderCenter();

        List<Node> nodeGrid = NodeManager.Instance.GetNodes();

        if (Path == null || Path.Count == 0)
        {
            Path =
            Paths.AStar(
                CurrentNode,
                targetNode,
                nodeGrid,
                MoveBehavior.Stable
            );
        }
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
