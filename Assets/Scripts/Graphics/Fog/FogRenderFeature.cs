using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace ElusiveWorld.Core.Assets.Scripts.Graphics.Fog
{
    public class FogRenderFeature : ScriptableRendererFeature
    {
        FogPass fogPass;

        public override void Create()
            => fogPass = new FogPass(RenderPassEvent.BeforeRenderingPostProcessing);

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
            => renderer.EnqueuePass(fogPass);

        [System.Obsolete]
        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
            => fogPass.Setup(renderer.cameraColorTargetHandle);
    }

    public class FogPass : ScriptableRenderPass
    {
        static readonly string shaderPath = "PostEffect/Fog";
        static readonly string k_RenderTag = "Render Fog Effects";
        static readonly int MainTexId = Shader.PropertyToID("_MainTex");
        static readonly int TempTargetId = Shader.PropertyToID("_TempTargetFog");
        static readonly int FogDensity = Shader.PropertyToID("_FogDensity");
        static readonly int FogDistance = Shader.PropertyToID("_FogDistance");
        static readonly int FogColor = Shader.PropertyToID("_FogColor");
        static readonly int AmbientColor = Shader.PropertyToID("_AmbientColor");
        static readonly int FogNear = Shader.PropertyToID("_FogNear");
        static readonly int FogFar = Shader.PropertyToID("_FogFar");
        static readonly int FogAltScale = Shader.PropertyToID("_FogAltScale");
        static readonly int FogThinning = Shader.PropertyToID("_FogThinning");
        static readonly int NoiseScale = Shader.PropertyToID("_NoiseScale");
        static readonly int NoiseStrength = Shader.PropertyToID("_NoiseStrength");
        readonly Material fogMaterial;
        RenderTargetIdentifier currentTarget;
        Fog fog;

        public FogPass(RenderPassEvent evt)
        {
            renderPassEvent = evt;
            var shader = Shader.Find(shaderPath);
            if (shader == null)
            {
                Debug.LogError("Shader not found.");
                return;
            }
            fogMaterial = CoreUtils.CreateEngineMaterial(shader);
        }

        [System.Obsolete]
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (fogMaterial == null)
            {
                Debug.LogError("Material not created.");
                return;
            }

            if (!renderingData.cameraData.postProcessEnabled) return;

            var stack = VolumeManager.instance.stack;
            fog = stack.GetComponent<Fog>();
            if (fog == null) { return; }
            if (!fog.IsActive()) { return; }

            var cmd = CommandBufferPool.Get(k_RenderTag);
            Render(cmd, ref renderingData);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public void Setup(in RenderTargetIdentifier currentTarget) 
            => this.currentTarget = currentTarget;

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
            fogMaterial.SetFloat(FogDensity, fog.fogDensity.value);
            fogMaterial.SetFloat(FogDistance, fog.fogDistance.value);
            fogMaterial.SetColor(FogColor, fog.fogColor.value);
            fogMaterial.SetColor(AmbientColor, fog.ambientColor.value);
            fogMaterial.SetFloat(FogNear, fog.fogNear.value);
            fogMaterial.SetFloat(FogFar, fog.fogFar.value);
            fogMaterial.SetFloat(FogAltScale, fog.fogAltScale.value);
            fogMaterial.SetFloat(FogThinning, fog.fogThinning.value);
            fogMaterial.SetFloat(NoiseScale, fog.noiseScale.value);
            fogMaterial.SetFloat(NoiseStrength, fog.noiseStrength.value);

            var shaderPass = 0;
            cmd.SetGlobalTexture(MainTexId, source);
            cmd.GetTemporaryRT(destination, w, h, 0, FilterMode.Point, RenderTextureFormat.Default);
            cmd.Blit(source, destination);
            cmd.Blit(destination, source, fogMaterial, shaderPass);
        }
    }
}