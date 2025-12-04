using UnityEngine;
public class Pixelate : MonoBehaviour
{
    [System.Serializable]
    struct TransformSettings
    {
        public Transform RefTransform;
        public bool AutoRotationEnabled;
    }
    [System.Serializable]
    struct RendererOptions
    {
        public SpriteRenderer SpriteRenderer;
        public ParticleSystem ParticleSystem;
    }
    [SerializeField] GameObject pixelateChildrenPrefab;
    [SerializeField] Vector2 offset;
    [SerializeField] RendererOptions rendererOptions;
    [SerializeField] TransformSettings transformSettings;
    public Quaternion Rotation
    {
        get => _pixelateCamera.transform.rotation;
        set => _pixelateCamera.transform.rotation = value;
    }
    readonly int _resolution = 128;
    bool _wasVisible = false;
    GameObject _pixelateChildren;
    Camera _pixelateCamera;
    MeshRenderer _quadMesh;
    RenderTexture _renderTexture;
    Material _materialPrefab;
    Material _material;

    void Awake()
    {
        Cache();
        AssignPixelateLayer(_wasVisible);

        _material.SetTexture("_MainTex", _renderTexture);
        _quadMesh.material = _material;

        if (offset.magnitude != 0)
        {
            Vector3 cameraPos = _pixelateCamera.transform.position;
            _pixelateCamera.transform.position = new Vector3(cameraPos.x + offset.x, cameraPos.y + offset.y, cameraPos.z);
        }
        _pixelateCamera.targetTexture = _renderTexture;
        _pixelateCamera.Render();

        QuadLayerSetter _sortingSetter = _quadMesh.GetComponent<QuadLayerSetter>();
        if (rendererOptions.SpriteRenderer)
        {
            _sortingSetter.RendererToTrack = rendererOptions.SpriteRenderer;
        }
        else if (rendererOptions.ParticleSystem)
        {
            _sortingSetter.RendererToTrack = rendererOptions.ParticleSystem.GetComponent<ParticleSystemRenderer>();
        }
    }

    void Update()
    {
        bool _isVisible = Utils.VisibleToCamera(transform, Camera.main);

        if (_wasVisible != _isVisible)
        {
            AssignPixelateLayer(_isVisible);
        }
        _wasVisible = _isVisible;

        if (transformSettings.AutoRotationEnabled)
        {
            Rotation = Quaternion.Euler(0, 0, -2 * transformSettings.RefTransform.rotation.z);
        }
    }

    void Cache()
    {
        _pixelateChildren = Instantiate(pixelateChildrenPrefab, transform);
        _pixelateCamera = _pixelateChildren.GetComponentInChildren<Camera>();
        _quadMesh = _pixelateChildren.GetComponentInChildren<MeshRenderer>();
        _materialPrefab = ResourceLoader.Instance.GetMaterial("TransparentRenderGraph");
        _material = Instantiate(_materialPrefab);
        _wasVisible = Utils.VisibleToCamera(transform, Camera.main);

        CreateRenderTexture();
    }

    void AssignPixelateLayer(bool isVisible)
    {
        if (isVisible)
        {
            int _assignedLayer = PixelateLayerManager.Instance.AssignUnusedLayer(gameObject);

            if (_assignedLayer == -1)
            {
                Debug.LogError("No available Pixelate layers!");
            }
            else
            {
                _pixelateCamera.cullingMask = (1 << _assignedLayer) | (1 << 3);
            }
        }
        else
        {
            PixelateLayerManager.Instance.ReleaseLayer(gameObject);
        }
    }

    public void SetSortingOrder(int order)
    {
        _quadMesh.sortingOrder = order;
    }

    public void RotateQuad(float x, float y, float z)
    {
        _quadMesh.transform.rotation = Quaternion.Euler(x, y, z);
    }

    void CreateRenderTexture()
    {
        var rtDesc = new RenderTextureDescriptor(_resolution, _resolution)
        {
            graphicsFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.R16G16B16A16_SFloat,
            depthBufferBits = 24,
            dimension = UnityEngine.Rendering.TextureDimension.Tex2D,
            msaaSamples = 1,
            mipCount = 1,
        };
        _renderTexture = new RenderTexture(rtDesc)
        {
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp,
        };
        _renderTexture.Create();
    }

    void CreateMaterial()
    {
        _material = new Material(Shader.Find("Universal Render Pipeline/Unlit"))
        {
            renderQueue = 3100
        };
        _material.SetFloat("_Surface", 1f);
        _material.SetTexture("_BaseMap", _renderTexture);
        _material.SetFloat("_AlphaClip", 0f);

        _material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
    }
}
