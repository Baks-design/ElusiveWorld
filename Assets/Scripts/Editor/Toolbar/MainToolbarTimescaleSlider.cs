using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.UIElements;

namespace ElusiveWorld.Core.Editor.Assets.Scripts.Editor.Toolbar
{
    public class MainToolbarTimescaleSlider
    {
        const float k_minTimeScale = 0.1f, k_maxTimeScale = 3f;

        [MainToolbarElement("TimeScale/Slider", defaultDockPosition = MainToolbarDockPosition.Middle)]
        public static MainToolbarElement TimeSlider()
        {
            MainToolbarElementStyler.StyleElement<VisualElement>("TimeScale/Slider", (element) =>
            {
                element.style.paddingRight = 10f;
            });

            return new MainToolbarSlider(
                new MainToolbarContent("Time Scale", "Time Scale"),
                Time.timeScale,
                k_minTimeScale,
                k_maxTimeScale,
                value => Time.timeScale = value
            );
        }
    }
}