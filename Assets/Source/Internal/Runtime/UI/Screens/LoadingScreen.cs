using UnityEngine;
using UnityEngine.UI;
using Unity.Cinemachine;
using ElusiveWorld.Core.Assets.Scripts.Utils.Extensions;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Services;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Updates.Variable;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Updates.Variable.Interfaces;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Events;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.UI
{
    public class LoadingScreen : MonoBehaviour, IUpdate, IService
    {
        [SerializeField] Image loadingBar;
        [SerializeField] GameObject loadingCanvas;
        [SerializeField] CinemachineCamera loadingCamera;
        [SerializeField] float fillSpeed = 0.5f;
        bool isLoading;
        float targetProgress;
        EventBinding<LoadingScreenEvent> loadingScreenEventBinding;

        void OnEnable()
        {
            UpdateManager.RegisterUpdate(this);

            loadingScreenEventBinding = new EventBinding<LoadingScreenEvent>(HandleLoadingScreenEvent);
            EventBus<LoadingScreenEvent>.Register(loadingScreenEventBinding);
        }

        void OnDisable()
        {
            EventBus<LoadingScreenEvent>.Deregister(loadingScreenEventBinding);

            UpdateManager.UnregisterUpdate(this);
        }

        void HandleLoadingScreenEvent(LoadingScreenEvent loadingScreenEvent)
        {
            targetProgress += loadingScreenEvent.targetProgress;
            loadingBar.fillAmount += loadingScreenEvent.currentProgress;
            EnableLoadingCanvas(loadingScreenEvent.enableCanvas);
        }

        void IUpdate.Update() => LoadingHandler();

        void LoadingHandler()
        {
            if (!isLoading) return;
            var currentFillAmount = loadingBar.fillAmount;
            var progressDifference = Mathf.Abs(currentFillAmount - targetProgress);
            var dynamicFillSpeed = progressDifference * fillSpeed;
            loadingBar.fillAmount = currentFillAmount.ExpDecay(targetProgress, dynamicFillSpeed, Time.deltaTime);
        }

        void EnableLoadingCanvas(bool enableCanvas)
        {
            isLoading = enableCanvas;
            loadingCanvas.SetActive(enableCanvas);
            loadingCamera.gameObject.SetActive(enableCanvas);
        }
    }
}