using UnityEngine;

namespace Assets.Scripts.Internal.Runtime.Core.Systems.Interaction
{
    public interface IHoverable
    {
        string Tooltip { get; set; }
        Transform TooltipTransform { get; }

        void OnHoverStart(Material hoverMat);
        void OnHoverEnd();
    }
}
