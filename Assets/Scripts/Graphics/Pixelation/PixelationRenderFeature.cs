using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace ElusiveWorld.Core.Assets.Scripts.Graphics.Pixelation
{
    public class PixelationRenderFeature : ScriptableRendererFeature
    {
        PixelationPass pixelationPass;

        public override void Create()
            => pixelationPass = new PixelationPass(RenderPassEvent.BeforeRenderingPostProcessing);

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
            => renderer.EnqueuePass(pixelationPass);

        [System.Obsolete]
        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
            => pixelationPass.Setup(renderer.cameraColorTargetHandle);
    }

    public class PixelationPass : ScriptableRenderPass
    {
        static readonly string shaderPath = "PostEffect/Pixelation";
        static readonly string k_RenderTag = "Render Pixelation Effects";
        static readonly int MainTexId = Shader.PropertyToID("_MainTex");
        static readonly int TempTargetId = Shader.PropertyToID("_TempTargetPixelation");
        static readonly int WidthPixelation = Shader.PropertyToID("_WidthPixelation");
        static readonly int HeightPixelation = Shader.PropertyToID("_HeightPixelation");
        static readonly int ColorPrecison = Shader.PropertyToID("_ColorPrecision");
        readonly Material pixelationMaterial;
        Pixelation pixelation;
        RenderTargetIdentifier currentTarget;

        public PixelationPass(RenderPassEvent evt)
        {
            renderPassEvent = evt;
            var shader = Shader.Find(shaderPath);
            if (shader == null)
            {
                Debug.LogError("Shader not found.");
                return;
            }
            pixelationMaterial = CoreUtils.CreateEngineMaterial(shader);
        }

        [System.Obsolete]
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (pixelationMaterial == null)
            {
                Debug.LogError("Material not created.");
                return;
            }

            if (!renderingData.cameraData.postProcessEnabled) return;

            var stack = VolumeManager.instance.stack;
            pixelation = stack.GetComponent<Pixelation>();
            if (pixelation == null) { return; }
            if (!pixelation.IsActive()) { return; }

            var cmd = CommandBufferPool.Get(k_RenderTag);
            Render(cmd, ref renderingData);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public void Setup(in RenderTargetIdentifier currentTarget) => this.currentTarget = currentTarget;

        void Render(CommandBuffer cmd, ref RenderingData renderingData)
        {
            ref var cameraData = ref renderingData.cameraData;
            var source = currentTarget;
            var destination = TempTargetId;

            //getting camera width and height 
            var w = cameraData.camera.scaledPixelWidth;
            var h = cameraData.camera.scaledPixelHeight;

            //setting parameters here 
            cameraData.camera.depthTextureMode = cameraData.camera.depthTextureMode | DepthTextureMode.Depth;
            pixelationMaterial.SetFloat(WidthPixelation, pixelation.widthPixelation.value);
            pixelationMaterial.SetFloat(HeightPixelation, pixelation.heightPixelation.value);
            pixelationMaterial.SetFloat(ColorPrecison, pixelation.colorPrecision.value);

            var shaderPass = 0;
            cmd.SetGlobalTexture(MainTexId, source);
            cmd.GetTemporaryRT(destination, w, h, 0, FilterMode.Point, RenderTextureFormat.Default);
            cmd.Blit(source, destination);
            cmd.Blit(destination, source, pixelationMaterial, shaderPass);
        }
    }
}