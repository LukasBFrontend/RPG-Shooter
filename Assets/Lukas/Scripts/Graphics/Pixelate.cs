using UnityEngine;
public class Pixelate : MonoBehaviour
{
    [SerializeField] private int resolution;
    [SerializeField] GameObject pixelateChildrenPrefab;
    [SerializeField] Vector2 offset;
    Material materialPrefab;
    GameObject pixelateChildren;
    RenderTexture renderTexture;
    Material material;

    Camera pixelateCamera;
    MeshRenderer quadMesh;


    void Awake()
    {
        pixelateChildren = Instantiate(pixelateChildrenPrefab, transform);
        CreateRenderTexture();
        materialPrefab = ResourceManager.Instance.GetMaterial("TransparentRenderGraph");
        material = Instantiate(materialPrefab);
        material.SetTexture("_MainTex", renderTexture);

        pixelateCamera = pixelateChildren.GetComponentInChildren<Camera>();
        quadMesh = pixelateChildren.GetComponentInChildren<MeshRenderer>();


        int assignedLayer = PixelateLayerManager.Instance.AssignUnusedLayer(gameObject);
        if (assignedLayer == -1)
        {
            Debug.LogError("No available Pixelate layers!");
        }
        else
        {
            pixelateCamera.cullingMask = 1 << assignedLayer;
        }

        pixelateCamera.targetTexture = renderTexture;
        Vector3 cameraPos = pixelateCamera.transform.position;
        pixelateCamera.transform.position = new Vector3(cameraPos.x + offset.x, cameraPos.y + offset.y, cameraPos.z);
        pixelateCamera.Render();

        quadMesh.material = material;
        quadMesh.gameObject.layer = 3;
    }

    void Update()
    {
        //SetZLayer();
    }

    public Quaternion rotation
    {
        get => pixelateCamera.transform.rotation;
        set => pixelateCamera.transform.rotation = value;
    }

    public void SetSortingOrder(int order)
    {
        quadMesh.sortingOrder = order;
    }

    public void RotateQuad(float x, float y, float z)
    {
        quadMesh.transform.rotation = Quaternion.Euler(x, y, z);
    }

    private void CreateRenderTexture()
    {
        var rtDesc = new RenderTextureDescriptor(resolution, resolution)
        {
            graphicsFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.R16G16B16A16_SFloat, // HDR safe
            depthBufferBits = 24,
            dimension = UnityEngine.Rendering.TextureDimension.Tex2D,
            msaaSamples = 1,
            mipCount = 1,
        };
        renderTexture = new RenderTexture(rtDesc)
        {
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp,
        };
        renderTexture.Create();
    }

    private void CreateMaterial()
    {
        material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        material.SetFloat("_Surface", 1f);
        material.renderQueue = 3100;
        material.SetTexture("_BaseMap", renderTexture);
        material.SetFloat("_AlphaClip", 0f);

        material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
    }

    /*     private bool SetZLayer()
        {
            if (!parent)
            {
                parent = transform.parent.gameObject;
                Debug.Log("Parent found: " + parent.name);
            }

            if (!parent)
            {
                Debug.Log("No parent found");
                return false;
            }
            else if (quadMesh.gameObject.layer == parent.layer)
            { return false; }
            quadMesh.gameObject.layer = parent.layer;
            Debug.Log(quadMesh.gameObject.name + " layer set to " + parent.name + "'s layer: " + parent.layer);
            return true;
        } */
}
