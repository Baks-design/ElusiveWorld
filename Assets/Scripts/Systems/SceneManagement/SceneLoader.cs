using ElusiveWorld.Core.Assets.Scripts.Systems.Game;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Services;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.SceneManagement
{
    public class SceneLoader : MonoBehaviour, IService
    {
        [SerializeField] SceneGroup[] sceneGroups;
        readonly SceneGroupManager manager = new();
        LoadingScreen loadingScreen;

        public void Initialize() => loadingScreen = IServiceLocator.Default.GetService<LoadingScreen>();

        public async Awaitable LoadSceneGroup(int index)
        {
            loadingScreen.SetProgress(0f);
            loadingScreen.TargetProgress = 1f;

            if (index < 0 || index >= sceneGroups.Length)
            {
                Debug.LogError("Invalid scene group index: " + index);
                return;
            }

            var progress = new LoadingProgress();
            progress.Progressed += target
                => loadingScreen.TargetProgress = Mathf.Max(target, loadingScreen.TargetProgress);

            loadingScreen.EnableLoadingCanvas(true);
            await manager.LoadScenes(sceneGroups[index], progress, false);
            loadingScreen.EnableLoadingCanvas(false);
        }

        public void Dispose() { }
    }
}