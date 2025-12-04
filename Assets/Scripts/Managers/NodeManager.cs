using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class NodeManager : Singleton<NodeManager>
{
    [System.Serializable]
    struct NodeMapInfo
    {
        public float DistanceBetweenNodes, ColliderCullingOffset;
        public int Columns, Rows;
    }

    [SerializeField] GameObject nodePrefab;
    [SerializeField] CompositeCollider2D tilemapCollider;
    [SerializeField] NodeMapInfo nodeMapInfo;
    List<IObstructive> _obstructives = new();
    Vector2 _startPoint, _size;
    List<List<Node>> _nodeGrid = new();

    void Start()
    {
        Generate();
        DisableObstructed();
    }

    public Node ClosestNode(Vector2 pos)
    {
        Node _closestCurrent = null;
        float _closestDistance = float.MaxValue;

        for (int x = 0; x < nodeMapInfo.Columns; x++)
        {
            for (int y = 0; y < nodeMapInfo.Rows; y++)
            {
                Node _node = _nodeGrid[x][y];

                if (!_node)
                {
                    continue;
                }

                float _distance = Vector2.Distance(pos, _node.transform.position);

                if (_distance < _closestDistance)
                {
                    _closestCurrent = _node;
                    _closestDistance = _distance;
                }
            }
        }
        return _closestCurrent;
    }

    public List<Node> GetNodes()
    {
        return _nodeGrid.SelectMany(_row => _row).ToList();
    }

    public void DestroyChildren()
    {
        Transform[] _children = gameObject.GetComponentsInChildren<Transform>();
        foreach (Transform _child in _children)
        {
            if (_child == transform)
            {
                continue;
            }
            DestroyImmediate(_child.gameObject);
        }
    }

    public void Generate()
    {
        _startPoint = transform.position;
        _size = new Vector2(nodeMapInfo.Columns - 1, nodeMapInfo.Rows - 1) * nodeMapInfo.DistanceBetweenNodes;
        Vector2 _topLeft = new(_startPoint.x - _size.x / 2, _startPoint.y + _size.y / 2);

        if (!nodePrefab)
        {
            Debug.LogError("Node prefab missing!");
            return;
        }

        Node[] _existingNodes = transform.GetComponentsInChildren<Node>();
        foreach (Node _node in _existingNodes)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                DestroyImmediate(_node.gameObject);
            else
                Destroy(_node.gameObject);
#else
            Destroy(_node.gameObject);
#endif
        }

        _nodeGrid.Clear();

        for (int x = 0; x < nodeMapInfo.Columns; x++)
        {
            _nodeGrid.Add(new List<Node>());
            for (int y = 0; y < nodeMapInfo.Rows; y++)
            {
                Vector3 _pos3D = (Vector3)(
                    _topLeft + new Vector2(x * nodeMapInfo.DistanceBetweenNodes, -y * nodeMapInfo.DistanceBetweenNodes)
                );
                Vector2 _pos2D = _pos3D;

                if (tilemapCollider.OverlapPoint(_pos2D))
                {
                    _nodeGrid[x].Add(null);
                    continue;
                }

                Vector2 _nearest = tilemapCollider.ClosestPoint(_pos2D);

                float _dx = Mathf.Abs(_nearest.x - _pos2D.x);
                float _dy = Mathf.Abs(_nearest.y - _pos2D.y);

                if (_dx < nodeMapInfo.ColliderCullingOffset && _dy < nodeMapInfo.ColliderCullingOffset)
                {
                    _nodeGrid[x].Add(null);
                    continue;
                }

                GameObject _nodeObj = Instantiate(nodePrefab, _pos3D, Quaternion.identity, transform);
                Node _node = _nodeObj.GetComponent<Node>();
                _nodeGrid[x].Add(_node);
            }
        }

        CreateConnectionsFull();
    }

    public void DisableObstructed()
    {
        _obstructives = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IObstructive>().ToList();

        foreach (Node node in _nodeGrid.SelectMany(_list => _list))
        {
            if (!node)
            {
                continue;
            }
            bool _isObstructed = false;

            foreach (IObstructive _obstructive in _obstructives)
            {
                if (_obstructive.OccupationBounds(.5f).Contains(node.transform.position))
                {
                    _isObstructed = true;
                    break;
                }
            }
            node.Disabled = _isObstructed;
            node.gameObject.SetActive(!_isObstructed);
        }
    }

    void CreateConnectionsFull()
    {
        for (int x = 0; x < nodeMapInfo.Columns; x++)
        {
            for (int y = 0; y < nodeMapInfo.Rows; y++)
            {
                Node _node = _nodeGrid[x][y];
                if (_node == null)
                {
                    continue;
                }

                _node.Connections = new List<Node>();

                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        if (dx == 0 && dy == 0) continue;

                        int _nx = x + dx;
                        int _ny = y + dy;

                        if (_nx >= 0 && _nx < nodeMapInfo.Columns && _ny >= 0 && _ny < nodeMapInfo.Rows)
                        {
                            Node _neighbor = _nodeGrid[_nx][_ny];
                            if (_neighbor != null)
                            {
                                _node.Connections.Add(_neighbor);
                            }
                        }
                    }
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(_startPoint, _size);
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(NodeManager))]
    public class NodeGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            NodeManager _generator = (NodeManager)target;
            if (GUILayout.Button("Generate Nodes"))
            {
                _generator.DestroyChildren();
                _generator.Generate();
            }
            if (GUILayout.Button("Destroy Nodes"))
            {
                _generator.DestroyChildren();
            }
        }
    }
#endif
}
