using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Interaction.Interfaces
{
    public interface IHoverable
    {
        string Tooltip { get; set; }
        Transform TooltipTransform { get; }

        void OnHoverStart(Material hoverMat);
        void OnHoverEnd();
    }
}
