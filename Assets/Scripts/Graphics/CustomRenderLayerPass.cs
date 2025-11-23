// CustomRenderLayer2DPass.cs
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RendererUtils;

public class CustomRenderLayer2DPass : ScriptableRenderPass
{
    string profilerTag;
    CustomRenderLayerFeature.Settings settings;

    ShaderTagId shaderTag = new ShaderTagId("UniversalForward"); // Default 2D shader pass

    public CustomRenderLayer2DPass(string profilerTag, CustomRenderLayerFeature.Settings settings)
    {
        this.profilerTag = profilerTag;
        this.settings = settings;
        // Choose a proper event; BeforeRendering ensures default 2D pass hasn’t drawn yet
        this.renderPassEvent = RenderPassEvent.BeforeRendering;
    }

    [System.Obsolete("Execute is only used in Compatibility Mode.")]
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        // Compatibility mode (optional)
        CommandBuffer cmd = CommandBufferPool.Get(profilerTag);

        var drawSettings = new DrawingSettings(shaderTag, new SortingSettings(renderingData.cameraData.camera))
        {
            perObjectData = renderingData.perObjectData
        };

        if (settings.overrideMaterial != null)
            drawSettings.overrideMaterial = settings.overrideMaterial;

        var filterSettings = new FilteringSettings(
            RenderQueueRange.all,
            settings.layerMask,              // Physics / normal layers
            settings.renderingLayerMask      // Custom Rendering Layer filter
        );

        var desc = new RendererListDesc(shaderTag, renderingData.cullResults, renderingData.cameraData.camera)
        {
            sortingCriteria = SortingCriteria.CommonOpaque,
            rendererConfiguration = renderingData.perObjectData,
            renderQueueRange = RenderQueueRange.all,
            layerMask = settings.layerMask,
            renderingLayerMask = settings.renderingLayerMask
        };

        var rendererList = context.CreateRendererList(desc);
        cmd.DrawRendererList(rendererList);

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        using (var builder = renderGraph.AddRasterRenderPass<PassData>("CustomRenderLayer2DPass", out var passData))
        {
            // Get camera and culling data
            var urpData = frameData.Get<UniversalRenderingData>();
            var camData = frameData.Get<UniversalCameraData>();
            var resourceData = frameData.Get<UniversalResourceData>();

            var drawSettings = new DrawingSettings(shaderTag, new SortingSettings(camData.camera))
            {
                perObjectData = urpData.perObjectData
            };

            if (settings.overrideMaterial != null)
                drawSettings.overrideMaterial = settings.overrideMaterial;

            var filterSettings = new FilteringSettings(
                RenderQueueRange.all,
                settings.layerMask,
                settings.renderingLayerMask
            );

            // RendererListDesc filters objects by Rendering Layer
            var desc = new RendererListDesc(shaderTag, urpData.cullResults, camData.camera)
            {
                sortingCriteria = SortingCriteria.CommonOpaque,
                rendererConfiguration = urpData.perObjectData,
                renderQueueRange = RenderQueueRange.all,
                layerMask = settings.layerMask,
                renderingLayerMask = settings.renderingLayerMask
            };

            passData.rendererList = renderGraph.CreateRendererList(desc);

            builder.UseRendererList(passData.rendererList);
            builder.SetRenderAttachment(resourceData.activeColorTexture, 0);
            builder.SetRenderAttachmentDepth(resourceData.activeDepthTexture, AccessFlags.Write);

            builder.SetRenderFunc((PassData data, RasterGraphContext rgc) =>
            {
                rgc.cmd.DrawRendererList(data.rendererList);
            });
        }
    }

    class PassData
    {
        public RendererListHandle rendererList;
    }
}
