using UnityEngine;
using UnityEngine.UI;
using Unity.Cinemachine;
using ElusiveWorld.Core.Assets.Scripts.Utils.Extensions;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Services;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Updates.Variable;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Updates.Variable.Interfaces;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.UI
{
    public class LoadingScreen : MonoBehaviour, IUpdate, IService
    {
        [SerializeField] Image loadingBar;
        [SerializeField] GameObject loadingCanvas;
        [SerializeField] CinemachineCamera loadingCamera;
        [SerializeField] float fillSpeed = 0.5f;
        bool isLoading;

        public float TargetProgress { get; set; } = 0f;

        void OnEnable() => UpdateManager.RegisterUpdate(this);

        void IUpdate.Update(float dt)
        {
            if (!isLoading) return;
            var currentFillAmount = loadingBar.fillAmount;
            var progressDifference = Mathf.Abs(currentFillAmount - TargetProgress);
            var dynamicFillSpeed = progressDifference * fillSpeed;
            SetProgress(currentFillAmount.ExpDecay(TargetProgress, dynamicFillSpeed, Time.deltaTime));
        }

        void OnDisable() => UpdateManager.UnregisterUpdate(this);

        public void SetProgress(float progress) => loadingBar.fillAmount = progress;

        public void EnableLoadingCanvas(bool enable = true)
        {
            isLoading = enable;
            loadingCanvas.SetActive(enable);
            loadingCamera.gameObject.SetActive(enable);
        }
    }
}