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
    public static List<Node> Straight(Node start, Node end)
    {
        Vector2 _startPos = start.transform.position;
        Vector2 _endPos = end.transform.position;
        Vector2 _xFirstPos = new(_endPos.x, _startPos.y);
        Vector2 _yFirstPos = new(_startPos.x, _endPos.y);

        Node _xFirstNode = NodeManager.Instance.ClosestNode(_xFirstPos);
        Node _yFirstNode = NodeManager.Instance.ClosestNode(_yFirstPos);

        const float THRESHOLD = 0.05f;
        float _xDist = Mathf.Abs((_endPos - _startPos).x);
        float _yDist = Mathf.Abs((_endPos - _startPos).y);

        if (_xDist > _yDist && _xFirstNode)
        {
            if (((Vector2)_xFirstNode.transform.position - _xFirstPos).magnitude < THRESHOLD)
            {
                return new() { start, end };
            }
        }
        else if (_yFirstNode)
        {
            if (((Vector2)_yFirstNode.transform.position - _yFirstPos).magnitude < THRESHOLD)
            {
                return new() { start, end };
            }
        }

        return new() { start, end };
    }
    public static List<Node> AStar(Node start, Node end, List<Node> nodeGrid, MoveBehavior behavior = MoveBehavior.Stable)
    {
        List<Node> _openSet = new();

        foreach (Node n in nodeGrid)
        {
            if (!n)
            {
                continue;
            }
            n.GScore = float.MaxValue;
        }

        start.GScore = 0;
        start.HScore = Vector2.Distance(start.transform.position, end.transform.position);
        _openSet.Add(start);


        while (_openSet.Count > 0)
        {
            int _lowestF = 0;

            for (int i = 1; i < _openSet.Count; i++)
            {
                float fA = _openSet[i].FScore();
                float fB = _openSet[_lowestF].FScore();

                if (fA < fB)
                {
                    _lowestF = i;
                }
                else if (Mathf.Approximately(fA, fB) && behavior == MoveBehavior.Stable)
                {
                    float aBias = DirectionBias(_openSet[i]);
                    float bBias = DirectionBias(_openSet[_lowestF]);
                    if (aBias < bBias)
                        _lowestF = i;
                }
            }

            Node _currentNode = _openSet[_lowestF];
            _openSet.Remove(_currentNode);

            if (_currentNode == end)
            {
                List<Node> path = new() { end };

                while (_currentNode != start)
                {
                    _currentNode = _currentNode.CameFrom;
                    path.Add(_currentNode);
                }

                path.Reverse();
                return path;
            }

            foreach (Node connectedNode in _currentNode.Connections)
            {
                if (_currentNode.Disabled)
                {
                    continue;
                }
                float _baseCost = Vector2.Distance(_currentNode.transform.position, connectedNode.transform.position);
                float _heldGScore = _currentNode.GScore + _baseCost;

                if (_heldGScore <= connectedNode.GScore)
                {
                    connectedNode.CameFrom = _currentNode;
                    connectedNode.GScore = _heldGScore;
                    connectedNode.HScore = Vector2.Distance(connectedNode.transform.position, end.transform.position);

                    if (!_openSet.Contains(connectedNode))
                    {
                        _openSet.Add(connectedNode);
                    }
                }
            }
        }

        return null;
    }

    static float DirectionBias(Node node)
    {
        if (node.CameFrom == null || node.CameFrom.CameFrom == null)
        {
            return 0f;
        }

        Vector2 _prevDir = (node.CameFrom.transform.position - node.CameFrom.CameFrom.transform.position).normalized;
        Vector2 _newDir = (node.transform.position - node.CameFrom.transform.position).normalized;

        return 1f - Vector2.Dot(_prevDir, _newDir);
    }
}
