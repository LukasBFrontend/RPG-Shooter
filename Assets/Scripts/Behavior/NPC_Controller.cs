using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(NPC))]
public class NPC_Controller : MonoBehaviour
{
    public Node currentNode;
    public Node playerNode;
    public List<Node> path = new();
    [HideInInspector] public Vector2 faceDir = Vector2.down;
    Rigidbody2D rb;
    BoxCollider2D col;

    bool isVisionChange, seesPlayer, pathQued = false;
    Vector2 lastTrackedPos;
    float moveThreshold = 2f;
    float refreshRate = 3f;
    float pathingTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();

        pathingTimer = 1 / refreshRate;
        SetNode(new(0, 0));
    }

    void Update()
    {
        pathingTimer -= Time.deltaTime;

        RayCastPlayer();

        if (path == null || path.Count == 0 || (!seesPlayer && isVisionChange))
        {
            CreatePath(PlayerConfig.Instance.Node);
        }

        if (pathingTimer <= 0f)
        {
            float playerMove = Vector2.Distance(lastTrackedPos, PlayerConfig.Instance.ColliderCenter());

            if (!seesPlayer && playerMove > moveThreshold)
            {
                pathingTimer = 1 / refreshRate;
                pathQued = true;
            }
        }

        if (seesPlayer)
        {
            Vector2 offset = PlayerToNPC().normalized * 1f;
            SetNode(offset);
            FollowDirect();
        }
        else
        {
            FollowPath();
        }
    }

    public void SetNode(Vector2 offset)
    {
        currentNode = NodeManager.Instance.ClosestNode((Vector2)transform.position + offset);
    }

    void SetFacing(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            GetComponent<NPC>().faceDir = new(Mathf.Sign(direction.x), 0);
        }
        else
        {
            GetComponent<NPC>().faceDir = new(0, Mathf.Sign(direction.y));
        }
    }
    public void FollowDirect()
    {
        Vector2 targetPos = PlayerConfig.Instance.ColliderCenter();
        Vector2 direction = PlayerToNPC().normalized;
        SetFacing(direction);

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            2 * Time.deltaTime
        );

        if (pathQued)
        {
            CreatePath(PlayerConfig.Instance.Node);
            pathQued = false;
        }
    }
    public void FollowPath()
    {
        if (path == null || path.Count == 0)
        {
            return;
        }

        int x = 0;

        Vector3 targetPos = new Vector3(path[x].transform.position.x, path[x].transform.position.y, 0);
        Vector2 direction = (targetPos - transform.position).normalized;
        SetFacing(direction);

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            2 * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, path[x].transform.position) < 0.1f)
        {
            currentNode = path[x];
            path.RemoveAt(x);

            if (pathQued)
            {
                CreatePath(PlayerConfig.Instance.Node);
                pathQued = false;
            }
        }
    }

    void RayCastPlayer()
    {
        Vector2 origin = seesPlayer ? transform.position : NodeManager.Instance.ClosestNode(transform.position).transform.position;
        Vector2 dir = PlayerToClosest().normalized;
        Vector2 perp = new(-dir.y, dir.x);

        float extents = Mathf.Max(col.bounds.extents.x, col.bounds.extents.y);
        float side = extents;
        float diagonal = Mathf.Sqrt(2 * Mathf.Pow(extents, 2));

        float axisAlign = Mathf.Abs(dir.x * dir.y) * 4f;
        axisAlign = Mathf.Clamp01(axisAlign);

        float offsetDist = Mathf.Lerp(side, diagonal, axisAlign);

        Vector2 offset = perp * offsetDist;

        float dist = PlayerToClosest().magnitude - offsetDist;
        if (dist <= 0) dist = 0.1f;

        RaycastHit2D[] hitsOne = Physics2D.RaycastAll(origin + offset, dir, dist);
        RaycastHit2D[] hitsTwo = Physics2D.RaycastAll(origin - offset, dir, dist);

        Vector2 endOne = hitsOne.Length > 0 ? hitsOne.Last().point : (origin + offset + dir * dist);
        Vector2 endTwo = hitsTwo.Length > 0 ? hitsTwo.Last().point : (origin - offset + dir * dist);

        Debug.DrawLine(origin + offset, endOne, Color.red);
        Debug.DrawLine(origin - offset, endTwo, Color.red);

        Debug.DrawLine(origin, origin + offset, Color.yellow);
        Debug.DrawLine(origin, origin - offset, Color.yellow);

        int obstacleCount = 0;

        for (int i = 0; i < hitsOne.Length; i++)
        {
            if (hitsOne[i].collider == col) continue;
            else if (!hitsOne[i].collider.CompareTag("Player")) obstacleCount++;
        }

        for (int i = 0; i < hitsTwo.Length; i++)
        {
            if (hitsTwo[i].collider == col) continue;
            else if (!hitsTwo[i].collider.CompareTag("Player")) obstacleCount++;
        }

        isVisionChange = !(seesPlayer == (obstacleCount == 0));
        seesPlayer = obstacleCount == 0;

        if (isVisionChange)
        {
            pathingTimer = 1 / refreshRate;
            pathQued = true;
        }
    }

    Vector2 PlayerToClosest()
    {
        Vector2 playerPos = PlayerConfig.Instance.ColliderCenter();
        Vector2 pos = NodeManager.Instance.ClosestNode(transform.position).transform.position;

        return playerPos - pos;
    }
    public Vector2 PlayerToNPC()
    {
        Vector2 playerPos = PlayerConfig.Instance.ColliderCenter();
        Vector2 pos = transform.position;

        return playerPos - pos;
    }
    public void CreatePath(Node targetNode)
    {
        if (path == null)
        {
            path = new();
            return;
        }
        path.Clear();

        lastTrackedPos = PlayerConfig.Instance.ColliderCenter();

        List<Node> nodeGrid = NodeManager.Instance.GetNodes();

        if (path == null || path.Count == 0)
        {
            path =
            Paths.AStar(
                currentNode,
                targetNode,
                nodeGrid,
                MoveBehavior.Stable,
                GetComponent<BoxCollider2D>()
            );
        }
    }
}
