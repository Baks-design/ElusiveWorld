using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace ElusiveWorld.Core.Assets.Scripts.Graphics.CRT
{
    public class CRTRenderFeature : ScriptableRendererFeature
    {
        CRTPass crtPass;

        public override void Create() => crtPass = new CRTPass(RenderPassEvent.BeforeRenderingPostProcessing);

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
            => renderer.EnqueuePass(crtPass);

        [System.Obsolete]
        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
            => crtPass.Setup(renderer.cameraColorTargetHandle);
    }

    public class CRTPass : ScriptableRenderPass
    {
        static readonly string shaderPath = "PostEffect/CRTShader";
        static readonly string k_RenderTag = "Render CRT Effects";
        static readonly int MainTexId = Shader.PropertyToID("_MainTex");
        static readonly int TempTargetId = Shader.PropertyToID("_TempTargetCRT");
        static readonly int ScanLinesWeight = Shader.PropertyToID("_ScanlinesWeight");
        static readonly int NoiseWeight = Shader.PropertyToID("_NoiseWeight");
        static readonly int ScreenBendX = Shader.PropertyToID("_ScreenBendX");
        static readonly int ScreenBendY = Shader.PropertyToID("_ScreenBendY");
        static readonly int VignetteAmount = Shader.PropertyToID("_VignetteAmount");
        static readonly int VignetteSize = Shader.PropertyToID("_VignetteSize");
        static readonly int VignetteRounding = Shader.PropertyToID("_VignetteRounding");
        static readonly int VignetteSmoothing = Shader.PropertyToID("_VignetteSmoothing");
        static readonly int ScanLinesDensity = Shader.PropertyToID("_ScanLinesDensity");
        static readonly int ScanLinesSpeed = Shader.PropertyToID("_ScanLinesSpeed");
        static readonly int NoiseAmount = Shader.PropertyToID("_NoiseAmount");
        static readonly int ChromaticRed = Shader.PropertyToID("_ChromaticRed");
        static readonly int ChromaticGreen = Shader.PropertyToID("_ChromaticGreen");
        static readonly int ChromaticBlue = Shader.PropertyToID("_ChromaticBlue");
        static readonly int GrilleOpacity = Shader.PropertyToID("_GrilleOpacity");
        static readonly int GrilleCounterOpacity = Shader.PropertyToID("_GrilleCounterOpacity");
        static readonly int GrilleResolution = Shader.PropertyToID("_GrilleResolution");
        static readonly int GrilleCounterResolution = Shader.PropertyToID("_GrilleCounterResolution");
        static readonly int GrilleBrightness = Shader.PropertyToID("_GrilleBrightness");
        static readonly int GrilleUvRotation = Shader.PropertyToID("_GrilleUvRotation");
        static readonly int GrilleUvMidPoint = Shader.PropertyToID("_GrilleUvMidPoint");
        static readonly int GrilleShift = Shader.PropertyToID("_GrilleShift");
        readonly Material crtMaterial;
        Crt m_Crt;
        RenderTargetIdentifier currentTarget;

        public CRTPass(RenderPassEvent evt)
        {
            renderPassEvent = evt;
            var shader = Shader.Find(shaderPath);
            if (shader == null)
            {
                Debug.LogError("Shader not found (crt).");
                return;
            }
            crtMaterial = CoreUtils.CreateEngineMaterial(shader);
        }

        [System.Obsolete]
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (crtMaterial == null)
            {
                Debug.LogError("Material not created.");
                return;
            }
            if (!renderingData.cameraData.postProcessEnabled) return;

            var stack = VolumeManager.instance.stack;
            m_Crt = stack.GetComponent<Crt>();
            if (m_Crt == null || !m_Crt.IsActive()) return;

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

            crtMaterial.SetFloat(ScanLinesWeight, m_Crt.scanlinesWeight.value);
            crtMaterial.SetFloat(NoiseWeight, m_Crt.noiseWeight.value);

            crtMaterial.SetFloat(ScreenBendX, m_Crt.screenBendX.value);
            crtMaterial.SetFloat(ScreenBendY, m_Crt.screenBendY.value);
            crtMaterial.SetFloat(VignetteAmount, m_Crt.vignetteAmount.value);
            crtMaterial.SetFloat(VignetteSize, m_Crt.vignetteSize.value);
            crtMaterial.SetFloat(VignetteRounding, m_Crt.vignetteRounding.value);
            crtMaterial.SetFloat(VignetteSmoothing, m_Crt.vignetteSmoothing.value);

            crtMaterial.SetFloat(ScanLinesDensity, m_Crt.scanlinesDensity.value);
            crtMaterial.SetFloat(ScanLinesSpeed, m_Crt.scanlinesSpeed.value);
            crtMaterial.SetFloat(NoiseAmount, m_Crt.noiseAmount.value);

            crtMaterial.SetVector(ChromaticRed, m_Crt.chromaticRed.value);
            crtMaterial.SetVector(ChromaticGreen, m_Crt.chromaticGreen.value);
            crtMaterial.SetVector(ChromaticBlue, m_Crt.chromaticBlue.value);

            crtMaterial.SetFloat(GrilleOpacity, m_Crt.grilleOpacity.value);
            crtMaterial.SetFloat(GrilleCounterOpacity, m_Crt.grilleCounterOpacity.value);
            crtMaterial.SetFloat(GrilleResolution, m_Crt.grilleResolution.value);
            crtMaterial.SetFloat(GrilleCounterResolution, m_Crt.grilleCounterResolution.value);
            crtMaterial.SetFloat(GrilleBrightness, m_Crt.grilleBrightness.value);
            crtMaterial.SetFloat(GrilleUvRotation, m_Crt.grilleUvRotation.value);
            crtMaterial.SetFloat(GrilleUvMidPoint, m_Crt.grilleUvMidPoint.value);
            crtMaterial.SetVector(GrilleShift, m_Crt.grilleShift.value);

            var shaderPass = 0;
            cmd.SetGlobalTexture(MainTexId, source);
            cmd.GetTemporaryRT(destination, w, h, 0, FilterMode.Point, RenderTextureFormat.Default);
            cmd.Blit(source, destination);
            cmd.Blit(destination, source, crtMaterial, shaderPass);
        }
    }
}