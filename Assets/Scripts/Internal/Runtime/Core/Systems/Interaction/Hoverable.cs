using UnityEngine;

namespace Assets.Scripts.Internal.Runtime.Core.Systems.Interaction
{
    [RequireComponent(typeof(MeshRenderer))]
    public class Hoverable : MonoBehaviour, IHoverable
    {
        public string tooltip;
        public Transform tooltipTransform;
        MeshRenderer meshRenderer;

        public Transform TooltipTransform => tooltipTransform;
        public Material MyMaterial { get; private set; }
        public string Tooltip
        {
            get => tooltip;
            set => tooltip = value;
        }

        protected virtual void Awake() => GetComponents();

        protected virtual void GetComponents()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            MyMaterial = meshRenderer.material;
        }

        public void OnHoverStart(Material hoverMat) => meshRenderer.material = hoverMat;

        public void OnHoverEnd() => meshRenderer.material = MyMaterial;
    }
}
