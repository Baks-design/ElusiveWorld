using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace ElusiveWorld.Core.Assets.Scripts.Graphics.Dithering
{
    public class DitheringRenderFeature : ScriptableRendererFeature
    {
        DitheringPass ditheringPass;

        public override void Create()
            => ditheringPass = new DitheringPass(RenderPassEvent.BeforeRenderingPostProcessing);

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
            => renderer.EnqueuePass(ditheringPass);

        [System.Obsolete]
        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
            => ditheringPass.Setup(renderer.cameraColorTargetHandle);
    }

    public class DitheringPass : ScriptableRenderPass
    {
        static readonly string shaderPath = "PostEffect/Dithering";
        static readonly string k_RenderTag = "Render Dithering Effects";
        static readonly int MainTexId = Shader.PropertyToID("_MainTex");
        static readonly int TempTargetId = Shader.PropertyToID("_TempTargetDithering");
        static readonly int PatternIndex = Shader.PropertyToID("_PatternIndex");
        static readonly int DitherThreshold = Shader.PropertyToID("_DitherThreshold");
        static readonly int DitherStrength = Shader.PropertyToID("_DitherStrength");
        static readonly int DitherScale = Shader.PropertyToID("_DitherScale");
        readonly Material ditheringMaterial;
        Dithering dithering;
        RenderTargetIdentifier currentTarget;

        public DitheringPass(RenderPassEvent evt)
        {
            renderPassEvent = evt;
            var shader = Shader.Find(shaderPath);
            if (shader == null)
            {
                Debug.LogError("Shader not found.");
                return;
            }
            ditheringMaterial = CoreUtils.CreateEngineMaterial(shader);
        }

        [System.Obsolete]
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (ditheringMaterial == null)
            {
                Debug.LogError("Material not created.");
                return;
            }

            if (!renderingData.cameraData.postProcessEnabled) return;

            var stack = VolumeManager.instance.stack;
            dithering = stack.GetComponent<Dithering>();
            if (dithering == null) { return; }
            if (!dithering.IsActive()) { return; }

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
            ditheringMaterial.SetInt(PatternIndex, dithering.patternIndex.value);
            ditheringMaterial.SetFloat(DitherThreshold, dithering.ditherThreshold.value);
            ditheringMaterial.SetFloat(DitherStrength, dithering.ditherStrength.value);
            ditheringMaterial.SetFloat(DitherScale, dithering.ditherScale.value);

            var shaderPass = 0;
            cmd.SetGlobalTexture(MainTexId, source);
            cmd.GetTemporaryRT(destination, w, h, 0, FilterMode.Point, RenderTextureFormat.Default);
            cmd.Blit(source, destination);
            cmd.Blit(destination, source, ditheringMaterial, shaderPass);
        }
    }
}