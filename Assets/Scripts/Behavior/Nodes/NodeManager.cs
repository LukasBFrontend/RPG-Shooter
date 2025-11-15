using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Linq;

public class NodeManager : Singleton<NodeManager>
{
    [SerializeField] GameObject nodePrefab;
    [SerializeField] CompositeCollider2D tilemapCollider;
    [SerializeField] float distanceBetweenNodes = 1f;
    [SerializeField] float colliderCullingOffset = .5f;
    [SerializeField] int columns = 4, rows = 4;

    Vector2 startPoint, size;
    private List<List<Node>> nodeGrid = new();

    private void Start()
    {
        Generate();
    }

    public Node ClosestNode(Vector2 position)
    {
        Node closestCurrent = null;
        float closestDistance = float.MaxValue;

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                Node node = nodeGrid[x][y];

                if (!node)
                {
                    continue;
                }

                float distance = Vector2.Distance(position, node.transform.position);

                if (distance < closestDistance)
                {
                    closestCurrent = node;
                    closestDistance = distance;
                }
            }
        }
        return closestCurrent;
    }

    public List<Node> GetNodes()
    {
        return nodeGrid.SelectMany(row => row).ToList();
    }
    public void DestroyChildren()
    {
        Transform[] children = gameObject.GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            if (child == transform)
            {
                continue;
            }
            DestroyImmediate(child.gameObject);
        }
    }
    public void Generate()
    {
        startPoint = transform.position;
        size = new Vector2(columns - 1, rows - 1) * distanceBetweenNodes;
        Vector2 topLeft = new(startPoint.x - size.x / 2, startPoint.y + size.y / 2);

        if (!nodePrefab)
        {
            Debug.LogError("Node prefab missing!");
            return;
        }

        Node[] existingNodes = transform.GetComponentsInChildren<Node>();
        foreach (Node n in existingNodes)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                DestroyImmediate(n.gameObject);
            else
                Destroy(n.gameObject);
#else
            Destroy(n.gameObject);
#endif
        }

        nodeGrid.Clear();

        for (int x = 0; x < columns; x++)
        {
            nodeGrid.Add(new List<Node>());
            for (int y = 0; y < rows; y++)
            {
                Vector2 position = topLeft + new Vector2(x * distanceBetweenNodes, -y * distanceBetweenNodes);

                Vector3 pos3D = new Vector3(position.x, position.y, 0f);
                Vector2 point2D = pos3D;

                if (tilemapCollider)
                {

                    if (tilemapCollider.OverlapPoint(point2D))
                    {
                        nodeGrid[x].Add(null);
                        continue;
                    }

                    Vector2 nearest = tilemapCollider.ClosestPoint(point2D);


                    float dx = Mathf.Abs(nearest.x - point2D.x);
                    float dy = Mathf.Abs(nearest.y - point2D.y);

                    if (dx < colliderCullingOffset && dy < colliderCullingOffset)
                    {
                        nodeGrid[x].Add(null);
                        continue;
                    }
                }




                GameObject nodeObj = Instantiate(nodePrefab, pos3D, Quaternion.identity, transform);
                Node node = nodeObj.GetComponent<Node>();
                nodeGrid[x].Add(node);
            }
        }

        CreateConnectionsFull();
        //CreateConnections();
    }

    void CreateConnections()
    {
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                Node node = nodeGrid[x][y];
                if (node == null) continue;

                node.connections = new List<Node>();

                if (x > 0 && nodeGrid[x - 1][y] != null)
                    node.connections.Add(nodeGrid[x - 1][y]);

                if (x < columns - 1 && nodeGrid[x + 1][y] != null)
                    node.connections.Add(nodeGrid[x + 1][y]);


                if (y > 0 && nodeGrid[x][y - 1] != null)
                    node.connections.Add(nodeGrid[x][y - 1]);

                if (y < rows - 1 && nodeGrid[x][y + 1] != null)
                    node.connections.Add(nodeGrid[x][y + 1]);
            }
        }
    }

    void CreateConnectionsFull()
    {
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                Node node = nodeGrid[x][y];
                if (node == null) continue;

                node.connections = new List<Node>();

                // Loop through all 8 directions
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        // Skip self
                        if (dx == 0 && dy == 0) continue;

                        int nx = x + dx;
                        int ny = y + dy;

                        // Check bounds
                        if (nx >= 0 && nx < columns && ny >= 0 && ny < rows)
                        {
                            Node neighbor = nodeGrid[nx][ny];
                            if (neighbor != null)
                                node.connections.Add(neighbor);
                        }
                    }
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(startPoint, size);
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(NodeManager))]
    public class NodeGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            NodeManager generator = (NodeManager)target;
            if (GUILayout.Button("Generate Nodes"))
            {
                generator.DestroyChildren();
                generator.Generate();
            }
            if (GUILayout.Button("Destroy Nodes"))
            {
                generator.DestroyChildren();
            }
        }
    }
#endif
}
