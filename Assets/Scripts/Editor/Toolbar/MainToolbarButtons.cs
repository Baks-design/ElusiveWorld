using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.UIElements;

namespace ElusiveWorld.Core.Editor.Assets.Scripts.Editor.Toolbar
{
    public class MainToolbarButtons
    {
        [MainToolbarElement("Project/Open Project Settings", defaultDockPosition = MainToolbarDockPosition.Middle)]
        public static MainToolbarElement ProjectSettingsButton()
            => new MainToolbarButton
            (
                new MainToolbarContent(EditorGUIUtility.IconContent("SettingsIcon").image as Texture2D),
                () => { SettingsService.OpenProjectSettings(); }
            );

        [MainToolbarElement("Timescale/Reset", defaultDockPosition = MainToolbarDockPosition.Middle)]
        public static MainToolbarElement ResetTimeScaleButton()
        {
            MainToolbarElementStyler.StyleElement<EditorToolbarButton>("Timescale/Reset", element =>
            {
                element.style.paddingLeft = 0f;
                element.style.paddingRight = 0f;
                element.style.marginLeft = 0f;
                element.style.marginRight = 0f;
                element.style.minWidth = 20f;
                element.style.maxWidth = 20f;

                var image = element.Q<Image>();
                if (image == null) return;
                image.style.width = 12f;
                image.style.height = 12f;
            });

            return new MainToolbarButton(
                new MainToolbarContent(EditorGUIUtility.IconContent("Refresh").image as Texture2D, "Reset"),
                () => { Time.timeScale = 1f; MainToolbar.Refresh("Timescale/Slider"); }
            );
        }
    }
}