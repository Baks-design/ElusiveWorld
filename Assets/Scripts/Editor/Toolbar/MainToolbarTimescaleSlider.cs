using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.UIElements;

namespace ElusiveWorld.Core.Editor.Assets.Scripts.Editor.Toolbar
{
    public class MainToolbarTimescaleSlider
    {
        const float k_minTimeScale = 0f, k_maxTimeScale = 5f;

        [MainToolbarElement("Timescale/Slider", defaultDockPosition = MainToolbarDockPosition.Middle)]
        public static MainToolbarElement TimeSlider()
        {
            MainToolbarElementStyler.StyleElement<VisualElement>("Timescale/Slider", (element) =>
            {
                element.style.paddingLeft = 10f;
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