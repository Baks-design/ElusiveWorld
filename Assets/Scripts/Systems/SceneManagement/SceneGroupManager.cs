using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Eflatun.SceneReference;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using ZLinq;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.SceneManagement
{
    public class SceneGroupManager
    {
        readonly AsyncOperationHandleGroup handleGroup = new(10);
        SceneGroup ActiveSceneGroup;

        public event Action<string> OnSceneLoaded = delegate { };
        public event Action<string> OnSceneUnloaded = delegate { };
        public event Action OnSceneGroupLoaded = delegate { };

        public async UniTask LoadScenes(
            SceneGroup group, IProgress<float> progress, bool reloadDupScenes = false,
            CancellationToken cancellationToken = default)
        {
            ActiveSceneGroup = group;
            var loadedScenes = new List<string>();

            await UnloadScenes(cancellationToken);

            var sceneCount = SceneManager.sceneCount;
            for (var i = 0; i < sceneCount; i++)
                loadedScenes.Add(SceneManager.GetSceneAt(i).name);

            var totalScenesToLoad = ActiveSceneGroup.Scenes.Count;
            var loadedCount = 0f;

            // Clear previous handles
            handleGroup.Handles.Clear();

            for (var i = 0; i < totalScenesToLoad; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var sceneData = group.Scenes[i];
                if (reloadDupScenes == false && loadedScenes.Contains(sceneData.Name))
                {
                    loadedCount++;
                    progress?.Report(loadedCount / totalScenesToLoad);
                    continue;
                }

                try
                {
                    if (sceneData.Reference.State == SceneReferenceState.Regular)
                    {
                        var operation = SceneManager.LoadSceneAsync(sceneData.Reference.Path, LoadSceneMode.Additive);
                        if (operation != null)
                            await operation.ToUniTask(cancellationToken: cancellationToken);
                    }
                    else if (sceneData.Reference.State == SceneReferenceState.Addressable)
                    {
                        var handle = Addressables.LoadSceneAsync(sceneData.Reference.Path, LoadSceneMode.Additive);
                        handleGroup.Handles.Add(handle);
                        await handle.ToUniTask(cancellationToken: cancellationToken);
                    }

                    OnSceneLoaded.Invoke(sceneData.Name);
                }
                catch (OperationCanceledException)
                {
                    Debug.Log($"Scene loading cancelled: {sceneData.Name}");
                    throw;
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to load scene {sceneData.Name}: {e}");
                }

                loadedCount++;
                progress?.Report(loadedCount / totalScenesToLoad);
            }

            var activeScene = SceneManager.GetSceneByName(ActiveSceneGroup.FindSceneNameByType(SceneType.ActiveScene));
            if (activeScene.IsValid())
                SceneManager.SetActiveScene(activeScene);

            OnSceneGroupLoaded.Invoke();
        }

        public async UniTask UnloadScenes(CancellationToken cancellationToken = default)
        {
            var scenes = new List<string>();
            var activeScene = SceneManager.GetActiveScene().name;

            var sceneCount = SceneManager.sceneCount;
            for (var i = sceneCount - 1; i > 0; i--)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var sceneAt = SceneManager.GetSceneAt(i);
                if (!sceneAt.isLoaded) continue;

                var sceneName = sceneAt.name;
                if (sceneName.Equals(activeScene) || sceneName == "Initiator") continue;

                var isAddressableScene = handleGroup.Handles.AsValueEnumerable().Any(h =>
                    h.IsValid() && h.Result.Scene.name == sceneName);

                if (isAddressableScene) continue;

                scenes.Add(sceneName);
            }

            // Unload regular scenes in parallel
            var unloadTasks = new List<UniTask>();
            foreach (var scene in scenes)
            {
                var operation = SceneManager.UnloadSceneAsync(scene);
                if (operation != null)
                {
                    unloadTasks.Add(operation.ToUniTask(cancellationToken: cancellationToken));
                    OnSceneUnloaded.Invoke(scene);
                }
            }

            // Wait for all regular scene unloads
            if (unloadTasks.Count > 0)
                await UniTask.WhenAll(unloadTasks);

            // Unload addressable scenes using stored handles
            var addressableUnloadTasks = new List<UniTask>();
            foreach (var handle in handleGroup.Handles)
            {
                if (handle.IsValid())
                {
                    var unloadHandle = Addressables.UnloadSceneAsync(handle);
                    addressableUnloadTasks.Add(unloadHandle.ToUniTask(cancellationToken: cancellationToken));
                }
            }

            if (addressableUnloadTasks.Count > 0)
                await UniTask.WhenAll(addressableUnloadTasks);

            handleGroup.Handles.Clear();

            await Resources.UnloadUnusedAssets().ToUniTask(cancellationToken: cancellationToken);
        }
    }
}