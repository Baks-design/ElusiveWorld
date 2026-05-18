using ElusiveWorld.Core.Assets.Scripts.Systems.Interaction.Interfaces;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Interaction.Types
{
    [RequireComponent(typeof(MeshRenderer))]
    public class Hoverable : MonoBehaviour, IHoverable
    {
        [SerializeField] MeshRenderer meshRenderer;
        
        [field: SerializeField] public Transform TooltipTransform { get; private set; }
        [field: SerializeField] public string Tooltip { get; set; }
        public Material MyMaterial { get; private set; }

        protected virtual void Awake()
        {
            TryGetComponent(out meshRenderer);
            MyMaterial = meshRenderer.sharedMaterial;
        }

        public void OnHoverStart(Material hoverMat) => meshRenderer.material = hoverMat;

        public void OnHoverEnd() => meshRenderer.material = MyMaterial;
    }
}
