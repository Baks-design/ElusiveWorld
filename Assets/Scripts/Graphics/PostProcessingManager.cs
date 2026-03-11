using ElusiveWorld.Core.Assets.Scripts.Graphics.CRT;
using ElusiveWorld.Core.Assets.Scripts.Graphics.Dithering;
using ElusiveWorld.Core.Assets.Scripts.Graphics.Fog;
using ElusiveWorld.Core.Assets.Scripts.Graphics.Pixelation;
using ElusiveWorld.Core.Assets.Scripts.Utils.Services;
using UnityEngine;
using UnityEngine.Rendering;

namespace ElusiveWorld.Core.Assets.Scripts.Graphics
{
    public class PostProcessingManager : MonoBehaviour, IService //TODO: Convert to recordgraph
    {
        [SerializeField] VolumeProfile profile;
        [SerializeField] CRTEffectController cRTEffectController;
        [SerializeField] DitheringController ditheringController;
        [SerializeField] FogController fogController;
        [SerializeField] PixelationController pixelationController;

        public void Initialize()
        {
            cRTEffectController.Initialize(profile);
            ditheringController.Initialize(profile);
            fogController.Initialize(profile);
            pixelationController.Initialize(profile);
        }

        void LateUpdate()
        {
            cRTEffectController.Update();
            ditheringController.Update();
            fogController.Update();
            pixelationController.Update();
        }

        public void Dispose() { }
    }
}