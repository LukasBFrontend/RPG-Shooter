using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public enum MoveBehavior
{
    Default,
    Stable
}
public static class Paths
{
    public static List<Node> Straight(Node start, Node end, List<Node> nodeGrid)
    {
        Vector2 startPos = start.transform.position;
        Vector2 endPos = end.transform.position;
        Vector2 xFirstPos = new(endPos.x, startPos.y);
        Vector2 yFirstPos = new(startPos.x, endPos.y);

        Node xFirstNode = NodeManager.Instance.ClosestNode(xFirstPos);
        Node yFirstNode = NodeManager.Instance.ClosestNode(yFirstPos);

        float threshold = 0.05f;
        float xDist = Mathf.Abs((endPos - startPos).x);
        float yDist = Mathf.Abs((endPos - startPos).y);

        if (xDist > yDist && xFirstNode)
        {
            if (((Vector2)xFirstNode.transform.position - xFirstPos).magnitude < threshold)
            {
                return new() { start, end };
            }
        }
        else if (yFirstNode)
        {
            if (((Vector2)yFirstNode.transform.position - yFirstPos).magnitude < threshold)
            {
                return new() { start, end };
            }
        }
        else
        {
            // try going halfways
        }

        return new() { start, end };
    }
    public static List<Node> AStar(Node start, Node end, List<Node> nodeGrid, MoveBehavior behavior = MoveBehavior.Stable, Collider2D selfCollider = null)
    {
        List<Node> openSet = new();

        foreach (Node n in nodeGrid)
        {
            if (!n)
            {
                continue;
            }
            n.gScore = float.MaxValue;
        }

        start.gScore = 0;
        start.hScore = Vector2.Distance(start.transform.position, end.transform.position);
        openSet.Add(start);


        while (openSet.Count > 0)
        {

            int lowestF = 0;
            for (int i = 1; i < openSet.Count; i++)
            {
                float fA = openSet[i].FScore();
                float fB = openSet[lowestF].FScore();

                if (fA < fB)
                {
                    lowestF = i;
                }
                else if (Mathf.Approximately(fA, fB) && behavior == MoveBehavior.Stable)
                {
                    float aBias = DirectionBias(openSet[i]);
                    float bBias = DirectionBias(openSet[lowestF]);
                    if (aBias < bBias)
                        lowestF = i;
                }
            }

            Node currentNode = openSet[lowestF];
            openSet.Remove(currentNode);

            if (currentNode == end)
            {
                List<Node> path = new List<Node> { end };

                while (currentNode != start)
                {
                    currentNode = currentNode.cameFrom;
                    path.Add(currentNode);
                }

                path.Reverse();
                return path;
            }

            foreach (Node connectedNode in currentNode.connections)
            {
                float baseCost = Vector2.Distance(currentNode.transform.position, connectedNode.transform.position);
                float heldGScore = currentNode.gScore + baseCost;

                if (currentNode == start)
                {
                    // --- SMART FIRST MOVE BIAS using raycast (ignoring self hits) ---
                    Vector2 origin = start.transform.position;
                    Vector2 targetPos = end.transform.position;
                    Vector2 toGoal = (targetPos - origin).normalized;
                    Vector2 toNeighbor = ((Vector2)connectedNode.transform.position - origin).normalized;
                    float maxDistance = Vector2.Distance(origin, targetPos);

                    RaycastHit2D[] hits = Physics2D.RaycastAll(origin, toGoal, maxDistance);
                    RaycastHit2D? firstValidHit = null;

                    foreach (var hit in hits)
                    {
                        if (hit.collider == null) continue;
                        if (selfCollider != null && hit.collider == selfCollider) continue;
                        firstValidHit = hit;
                        break;
                    }

                    if (firstValidHit.HasValue)
                    {

                        Vector2 hitDir = (firstValidHit.Value.point - origin).normalized;

                        float alignmentWithBlocked = Mathf.Abs(Vector2.Dot(toNeighbor, hitDir));
                        //heldGScore += alignmentWithBlocked * 50f;
                    }
                }
                else
                {
                    Vector2 prevDir = (currentNode.transform.position - currentNode.cameFrom.transform.position).normalized;
                    Vector2 newDir = (connectedNode.transform.position - currentNode.transform.position).normalized;

                    if (Vector2.Dot(prevDir, newDir) < 0.99f)
                    {
                        //heldGScore += 5f;
                    }
                }

                if (heldGScore <= connectedNode.gScore)
                {
                    connectedNode.cameFrom = currentNode;
                    connectedNode.gScore = heldGScore;
                    connectedNode.hScore = Vector2.Distance(connectedNode.transform.position, end.transform.position);

                    if (!openSet.Contains(connectedNode))
                        openSet.Add(connectedNode);
                }
            }
        }

        return null;
    }

    private static float DirectionBias(Node node)
    {
        if (node.cameFrom == null || node.cameFrom.cameFrom == null)
            return 0f;

        Vector2 prevDir = (node.cameFrom.transform.position - node.cameFrom.cameFrom.transform.position).normalized;
        Vector2 newDir = (node.transform.position - node.cameFrom.transform.position).normalized;

        return 1f - Vector2.Dot(prevDir, newDir);
    }
}
