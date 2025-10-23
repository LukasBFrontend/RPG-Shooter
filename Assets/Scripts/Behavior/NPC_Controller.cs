using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(BoxCollider2D))]
public class NPC_Controller : MonoBehaviour
{
    public Node currentNode;
    public Node playerNode;
    public List<Node> path = new();
    [HideInInspector] public Vector2 faceDir = Vector2.down;
    Rigidbody2D rb;
    BoxCollider2D col;

    bool isVisionChange, seesPlayer, pathQued = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        SetNode();
        RayCastPlayer();

        if (path == null || path.Count == 0)
        {
            ResetPath();
            CreatePath(PlayerConfig.Instance.Node, seesPlayer);
        }

        FollowPath();
        UpdateRotation();
    }

    public void SetNode()
    {
        currentNode = NodeManager.Instance.ClosestNode(transform.position);
    }

    public void UpdateRotation()
    {
        float angle = Mathf.Atan2(faceDir.y, faceDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new(0, 0, angle + 90));
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
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            faceDir = new(Mathf.Sign(direction.x), 0);
        }
        else
        {
            faceDir = new(0, Mathf.Sign(direction.y));
        }


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
                ResetPath();
                CreatePath(PlayerConfig.Instance.Node, seesPlayer);
                pathQued = false;
            }
        }
    }

    void RayCastPlayer()
    {
        Vector2 origin = transform.position;
        Vector2 dir = PlayerToNPC().normalized;
        Vector2 perp = new(-dir.y, dir.x);

        float extents = Mathf.Max(col.bounds.extents.x, col.bounds.extents.y);
        float offsetDist = Mathf.Sqrt(2 * Mathf.Pow(extents, 2));
        Vector2 offset = perp * offsetDist;

        float dist = PlayerToNPC().magnitude - offsetDist;
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

        if (isVisionChange) pathQued = true;

        if (isVisionChange) Debug.Log("Vision changed");
        Debug.Log("seesPlayer: " + seesPlayer);
    }

    Vector2 PlayerToNPC()
    {
        Vector2 playerPos = PlayerConfig.Instance.ColliderCenter();
        Vector2 pos = transform.position;

        return playerPos - pos;
    }
    public void ResetPath()
    {
        if (path == null)
        {
            path = new();
            return;
        }
        path.Clear();
    }
    public void CreatePath(Node targetNode, bool followDirect)
    {
        if (path.Count > 0)
        {
            return;
        }

        List<Node> nodeGrid = NodeManager.Instance.GetNodes();

        if (path == null || path.Count == 0)
        {
            path =
            !followDirect ?
            Paths.AStar(
                currentNode,
                targetNode,
                NodeManager.Instance.GetNodes(),
                MoveBehavior.Stable,
                GetComponent<BoxCollider2D>()
            )
            :
            Paths.Straight(
                currentNode,
                targetNode,
                NodeManager.Instance.GetNodes()
            );
        }
    }
}
