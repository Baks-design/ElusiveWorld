using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Events;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Services;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.SceneManagement
{
    public class SceneLoader : MonoBehaviour, IService
    {
        [SerializeField] SceneGroup[] sceneGroups;
        readonly SceneGroupManager manager = new();

        public async Awaitable LoadSceneGroup(int index)
        {            
            EventBus<LoadingScreenEvent>.Raise(new LoadingScreenEvent
            {
                currentProgress = 0f,
                targetProgress = 1f
            });

            if (index < 0 || index >= sceneGroups.Length)
            {
                Debug.LogError($"Invalid scene group index: {index}");
                return;
            }

            var progress = new LoadingProgress();
            var currentProgress = new LoadingScreenEvent();
            progress.Progressed += target => currentProgress.targetProgress = Mathf.Max(target, currentProgress.targetProgress);

            EventBus<LoadingScreenEvent>.Raise(new LoadingScreenEvent { enableCanvas = true });
            await manager.LoadScenes(sceneGroups[index], progress, false);
            EventBus<LoadingScreenEvent>.Raise(new LoadingScreenEvent { enableCanvas = false });
        }
    }
}