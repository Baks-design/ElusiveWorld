using UnityEngine;

namespace VHS
{
    [RequireComponent(typeof(MeshRenderer))]
    public class Hoverable : MonoBehaviour, IHoverable
    {
        [Header("Settings")]
        public string tooltip;
        public Transform tooltipTransform;
        MeshRenderer meshRenderer;

        public Material MyMaterial { get; private set; }
        public string Tooltip { get => tooltip; set => tooltip = value; }
        public Transform TooltipTransform => tooltipTransform;

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
