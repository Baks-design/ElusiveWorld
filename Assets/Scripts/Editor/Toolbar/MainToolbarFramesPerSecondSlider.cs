using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.UIElements;

namespace ElusiveWorld.Core.Editor.Assets.Scripts.Editor.Toolbar
{
    public class MainToolbarFramesPerSecondSlider
    {
        const float k_minFramesPerSecond = 1f, k_maxFramesPerSecond = 120f;

        [MainToolbarElement("FramesPerSecond/Slider", defaultDockPosition = MainToolbarDockPosition.Middle)]
        public static MainToolbarElement FPSSlider()
        {
            MainToolbarElementStyler.StyleElement<VisualElement>("FramesPerSecond/Slider", (element) =>
            {
                element.style.paddingRight = 20f;
            });

            return new MainToolbarSlider(
                new MainToolbarContent("Frames Per Second", "Frames Per Second"),
                Application.targetFrameRate,
                k_minFramesPerSecond,
                k_maxFramesPerSecond,
                value => Application.targetFrameRate = (int)value
            );
        }
    }
}