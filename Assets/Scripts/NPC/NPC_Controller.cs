using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(NPC))]
public class NPC_Controller : MonoBehaviour
{
    [SerializeField] Collider2D col;
    [HideInInspector] public Node CurrentNode;
    [HideInInspector] public Node PlayerNode;
    [HideInInspector] public List<Node> Path = new();
    [HideInInspector] public Vector2 FaceDir = Vector2.down;
    bool _isVisionChange, _seesPlayer, _pathQued = false;
    Vector2 _lastTrackedPos;
    float _pathingTimer = 0f;
    readonly string[] RAYCAST_IGNORE_TAGS = { "Player", "Player Interact", "Player Trigger", "Hit Collider" };
    const float MOVE_THRESHOLD = 2f;
    const float REFRESH_RATE = 3f;

    void Start()
    {
        _pathingTimer = 1 / REFRESH_RATE;
        SetNode(new(0, 0));
    }

    void Update()
    {
        _pathingTimer -= Time.deltaTime;

        RayCastPlayer();

        if (Path == null || Path.Count == 0 || (!_seesPlayer && _isVisionChange))
        {
            CreatePath(PlayerConfig.Instance.Node());
        }

        if (_pathingTimer <= 0f)
        {
            float _playerMove = Vector2.Distance(_lastTrackedPos, PlayerConfig.Instance.ColliderCenter());

            if (!_seesPlayer && _playerMove > MOVE_THRESHOLD)
            {
                _pathingTimer = 1 / REFRESH_RATE;
                _pathQued = true;
            }
        }

        if (_seesPlayer)
        {
            Vector2 _offset = PlayerToNPC().normalized * 1f;
            SetNode(_offset);
            FollowDirect();
        }
        else
        {
            FollowPath();
        }
    }

    public void SetNode(Vector2 _offset)
    {
        CurrentNode = NodeManager.Instance.ClosestNode((Vector2)transform.position + _offset);
    }

    void SetFacing(Vector2 _direction)
    {
        if (Mathf.Abs(_direction.x) > Mathf.Abs(_direction.y))
        {
            GetComponent<NPC>().FaceDir = new(Mathf.Sign(_direction.x), 0);
        }
        else
        {
            GetComponent<NPC>().FaceDir = new(0, Mathf.Sign(_direction.y));
        }
    }

    public void FollowDirect()
    {
        Vector2 _targetPos = PlayerConfig.Instance.ColliderCenter();
        Vector2 _direction = PlayerToNPC().normalized;
        SetFacing(_direction);

        transform.position = Vector3.MoveTowards(
            transform.position,
            _targetPos,
            2 * Time.deltaTime
        );

        if (_pathQued)
        {
            CreatePath(PlayerConfig.Instance.Node());
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
        SetFacing(_direction);

        transform.position = Vector3.MoveTowards(
            transform.position,
            _targetPos,
            2 * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, Path[x].transform.position) < 0.1f)
        {
            CurrentNode = Path[x];
            Path.RemoveAt(x);

            if (_pathQued)
            {
                CreatePath(PlayerConfig.Instance.Node());
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
        Vector2 _dir = PlayerToClosest().normalized;
        Vector2 _perp = new(-_dir.y, _dir.x);

        float _extents = Mathf.Max(col.bounds.extents.x, col.bounds.extents.y);
        float _side = _extents;
        float _diagonal = Mathf.Sqrt(2 * Mathf.Pow(_extents, 2));

        float _axisAlign = Mathf.Abs(_dir.x * _dir.y) * 4f;
        _axisAlign = Mathf.Clamp01(_axisAlign);

        float _offsetDist = Mathf.Lerp(_side, _diagonal, _axisAlign);

        Vector2 _offset = _perp * _offsetDist;

        float _dist = PlayerToClosest().magnitude - _offsetDist;
        if (_dist <= 0) _dist = 0.1f;

        RaycastHit2D[] _hitsOne = Physics2D.RaycastAll(_origin + _offset, _dir, _dist);
        RaycastHit2D[] _hitsTwo = Physics2D.RaycastAll(_origin - _offset, _dir, _dist);
        RaycastHit2D[] _hitsThree = Physics2D.RaycastAll(_origin, _dir, _dist);

        Vector2 _endOne = _hitsOne.Length > 0 ? _hitsOne.Last().point : (_origin + _offset + _dir * _dist);
        Vector2 _endTwo = _hitsTwo.Length > 0 ? _hitsTwo.Last().point : (_origin - _offset + _dir * _dist);
        Vector2 _endThree = _hitsThree.Length > 0 ? _hitsThree.Last().point : (_origin + _dir * _dist);

        Debug.DrawLine(_origin + _offset, _endOne, Color.red);
        Debug.DrawLine(_origin - _offset, _endTwo, Color.red);
        Debug.DrawLine(_origin, _endThree, Color.red);

        Debug.DrawLine(_origin, _origin + _offset, Color.yellow);
        Debug.DrawLine(_origin, _origin - _offset, Color.yellow);

        int _obstacleCount = 0;

        for (int i = 0; i < _hitsOne.Length; i++)
        {
            if (_hitsOne[i].collider == col)
            {
                continue;
            }
            else if (!RAYCAST_IGNORE_TAGS.Contains(_hitsOne[i].collider.tag))
            {
                _obstacleCount++;
            }
        }

        for (int i = 0; i < _hitsTwo.Length; i++)
        {
            if (_hitsTwo[i].collider == col)
            {
                continue;
            }
            else if (!RAYCAST_IGNORE_TAGS.Contains(_hitsTwo[i].collider.tag))
            {
                _obstacleCount++;
            }
        }

        for (int i = 0; i < _hitsThree.Length; i++)
        {
            if (_hitsThree[i].collider == col)
            {
                continue;
            }
            else if (!RAYCAST_IGNORE_TAGS.Contains(_hitsThree[i].collider.tag))
            {
                _obstacleCount++;
            }
        }

        _isVisionChange = !(_seesPlayer == (_obstacleCount == 0));
        _seesPlayer = _obstacleCount == 0;

        if (_isVisionChange)
        {
            _pathingTimer = 1 / REFRESH_RATE;
            _pathQued = true;
        }
    }

    Vector2 PlayerToClosest()
    {
        Vector2 _playerPos = PlayerConfig.Instance.ColliderCenter();
        Vector2 _pos = NodeManager.Instance.ClosestNode(transform.position).transform.position;

        return _playerPos - _pos;
    }

    public Vector2 PlayerToNPC()
    {
        Vector2 _playerPos = PlayerConfig.Instance.ColliderCenter();
        Vector2 _pos = transform.position;

        return _playerPos - _pos;
    }

    public void CreatePath(Node targetNode)
    {
        if (Path == null || CurrentNode == null)
        {
            Path = new();
            return;
        }
        Path.Clear();

        _lastTrackedPos = PlayerConfig.Instance.ColliderCenter();

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
}
