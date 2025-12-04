using UnityEngine;
using System.Collections.Generic;

public class Node : MonoBehaviour
{
    public Node CameFrom;
    public List<Node> Connections;
    public float GScore, HScore;
    public bool Disabled = false;

    public float FScore()
    {
        return GScore + HScore;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        if (Connections == null || Connections.Count <= 0)
        {
            return;
        }

        foreach (Node connection in Connections)
        {
            if (connection == null)
            {
                continue;
            }

            Vector3 _start = transform.position;
            Vector3 _end = connection.transform.position;
            Vector3 _direction = (_end - _start).normalized;
            Vector3 _perpendicular = Vector3.Cross(_direction, Vector3.forward).normalized * 0.05f;

            _start -= _perpendicular;
            _end -= _perpendicular;

            Gizmos.DrawLine(_start, _end);

            Vector3 _middlePoint = (_end - _start) / 2 + _start;

            float _arrowSize = 0.1f;

            Vector3 _arrowDir1 = Quaternion.Euler(0, 0, 180 - 45) * _direction * _arrowSize;
            Vector3 _arrowDir2 = Quaternion.Euler(0, 0, 180 + 45) * _direction * _arrowSize;

            Gizmos.DrawLine(_middlePoint, _middlePoint + _arrowDir1);
            Gizmos.DrawLine(_middlePoint, _middlePoint + _arrowDir2);
        }
    }
}
