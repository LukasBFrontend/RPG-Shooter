// CustomRenderLayerFeature.cs
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CustomRenderLayerFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public string passName = "CustomRenderLayerPass2D";
        public RenderPassEvent passEvent = RenderPassEvent.AfterRenderingOpaques;
        public LayerMask layerMask = ~0; // physics layer mask (optional if you want also filter by GameObject layer)
        public uint renderingLayerMask = 1; // rendering layer mask
        public Material overrideMaterial = null; // optional material override
    }

    public Settings settings = new Settings();
    CustomRenderLayer2DPass _pass;

    public override void Create()
    {
        _pass = new CustomRenderLayer2DPass(settings.passName, settings);
        _pass.renderPassEvent = settings.passEvent;
        _pass.renderPassEvent = RenderPassEvent.BeforeRendering;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(_pass);
    }

    protected override void Dispose(bool disposing)
    {
        // Clean up if needed
    }
}
