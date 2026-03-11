using UnityEngine;
using UnityEngine.UI;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Game
{
    public class LoadingScreen : MonoBehaviour 
    {
        [SerializeField] GameObject loadingPanel;
        [SerializeField] Slider loadingBarSlider;

        public void Show() => loadingPanel.SetActive(true);

        public void Hide() => loadingPanel.SetActive(false);

        public void SetLoadingBarPercent(float percent) 
            => loadingBarSlider.value += percent;
    }
}