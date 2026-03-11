using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using ElusiveWorld.Core.Assets.Scripts.Utils.Extensions;
using ElusiveWorld.Core.Assets.Scripts.Utils.Services;
using UnityEngine;
using UnityEngine.UI;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.SceneManagement
{
    public class SceneLoader : MonoBehaviour, IService
    {
        [SerializeField] Image loadingBar;
        [SerializeField] float fillSpeed = 0.5f;
        [SerializeField] Canvas loadingCanvas;
        [SerializeField] Camera loadingCamera;
        [SerializeField] SceneGroup[] sceneGroups;
        readonly SceneGroupManager sceneGroupManager = new();
        float targetProgress;
        bool isLoading;
        bool isLoadingScenes; 

        public SceneGroupManager SceneGroupManager => sceneGroupManager;

        public void Initialize() { }

        void Update()
        {
            if (!isLoading) return;

            var currentFillAmount = loadingBar.fillAmount;
            var progressDifference = Mathf.Abs(currentFillAmount - targetProgress);
            var dynamicFillSpeed = progressDifference * fillSpeed;
            loadingBar.fillAmount = currentFillAmount.ExpDecay(targetProgress, dynamicFillSpeed, Time.deltaTime);
        }

        public async UniTask LoadSceneGroup(int index, CancellationToken cancellationToken = default)
        {
            if (isLoadingScenes) return;

            if (index < 0 || index >= sceneGroups.Length)
            {
                Debug.LogError($"Invalid scene group index: {index}");
                return;
            }

            isLoadingScenes = true;
            loadingBar.fillAmount = 0f;
            targetProgress = 1f;

            var progress = new LoadingProgress();
            progress.Progressed += target => targetProgress = Mathf.Max(target, targetProgress);

            EnableLoadingCanvas(true);

            try
            {
                // Ensure canvas is ready
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                // Load scenes with cancellation support
                await sceneGroupManager.LoadScenes(sceneGroups[index], progress)
                    .AttachExternalCancellation(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Scene loading was cancelled");
            }
            finally
            {
                EnableLoadingCanvas(false);
                isLoadingScenes = false;
            }
        }

        void EnableLoadingCanvas(bool enable = true)
        {
            isLoading = enable;
            loadingCanvas.gameObject.SetActive(enable);
            loadingCamera.gameObject.SetActive(enable);
        }

        public void Dispose() { }
    }
}