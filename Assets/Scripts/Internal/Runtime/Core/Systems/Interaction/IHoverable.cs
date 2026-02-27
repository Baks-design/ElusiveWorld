using UnityEngine;

namespace VHS
{    
    public interface IHoverable
    {
        string Tooltip { get; set;}
        Transform TooltipTransform { get; }

        void OnHoverStart(Material hoverMat);
        void OnHoverEnd();
    }
}
