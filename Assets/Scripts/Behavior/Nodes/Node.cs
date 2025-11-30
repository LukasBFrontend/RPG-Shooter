using UnityEngine;
using System.Collections.Generic;

public class Node : MonoBehaviour
{
    public Node cameFrom;
    public List<Node> connections;
    public float gScore, hScore;
    public bool disabled = false;

    public float FScore()
    {
        return gScore + hScore;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        if (connections == null || connections.Count <= 0)
            return;

        foreach (Node connection in connections)
        {
            if (connection == null)
                continue;

            Vector3 start = transform.position;
            Vector3 end = connection.transform.position;
            Vector3 direction = (end - start).normalized;
            Vector3 perpendicular = Vector3.Cross(direction, Vector3.forward).normalized * 0.05f;

            start -= perpendicular;
            end -= perpendicular;

            Gizmos.DrawLine(start, end);

            Vector3 middlePoint = (end - start) / 2 + start;

            float arrowSize = 0.1f;

            Vector3 arrowDir1 = Quaternion.Euler(0, 0, 180 - 45) * direction * arrowSize;
            Vector3 arrowDir2 = Quaternion.Euler(0, 0, 180 + 45) * direction * arrowSize;

            Gizmos.DrawLine(middlePoint, middlePoint + arrowDir1);
            Gizmos.DrawLine(middlePoint, middlePoint + arrowDir2);
        }
    }
}
