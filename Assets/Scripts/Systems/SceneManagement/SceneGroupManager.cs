using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Eflatun.SceneReference;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
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

        public async UniTask LoadScenes(SceneGroup group, IProgress<float> progress, bool reloadDupScenes = false)
        {
            ActiveSceneGroup = group;
            var loadedScenes = new List<string>();

            await UnloadScenes();

            var sceneCount = SceneManager.sceneCount;
            for (var i = 0; i < sceneCount; i++)
                loadedScenes.Add(SceneManager.GetSceneAt(i).name);

            var totalScenesToLoad = ActiveSceneGroup.Scenes.Count;
            var loadTasks = new List<UniTask>();

            for (var i = 0; i < totalScenesToLoad; i++)
            {
                var sceneData = group.Scenes[i];
                if (reloadDupScenes == false && loadedScenes.Contains(sceneData.Name)) continue;

                if (sceneData.Reference.State == SceneReferenceState.Regular)
                {
                    var operation = SceneManager.LoadSceneAsync(sceneData.Reference.Path, LoadSceneMode.Additive);
                    loadTasks.Add(operation.ToUniTask());
                }
                else if (sceneData.Reference.State == SceneReferenceState.Addressable)
                {
                    var sceneHandle = Addressables.LoadSceneAsync(sceneData.Reference.Path, LoadSceneMode.Additive);
                    loadTasks.Add(sceneHandle.ToUniTask());
                }

                OnSceneLoaded.Invoke(sceneData.Name);

                progress?.Report(i / (float)totalScenesToLoad);
            }

            await UniTask.WhenAll(loadTasks);

            var activeScene = SceneManager.GetSceneByName(ActiveSceneGroup.FindSceneNameByType(SceneType.ActiveScene));
            if (activeScene.IsValid()) SceneManager.SetActiveScene(activeScene);

            OnSceneGroupLoaded.Invoke();
        }

        public async UniTask UnloadScenes()
        {
            var scenes = new List<string>();
            var activeScene = SceneManager.GetActiveScene().name;

            var sceneCount = SceneManager.sceneCount;
            for (var i = sceneCount - 1; i > 0; i--)
            {
                var sceneAt = SceneManager.GetSceneAt(i);
                if (!sceneAt.isLoaded) continue;

                var sceneName = sceneAt.name;
                if (sceneName.Equals(activeScene) || sceneName == "Initiator") continue;
                if (handleGroup.Handles.AsValueEnumerable().Any(h => h.IsValid() 
                    && h.Result.Scene.name == sceneName)) continue;

                scenes.Add(sceneName);
            }

            var operationGroup = new AsyncOperationGroup(scenes.Count);

            foreach (var scene in scenes)
            {
                var operation = SceneManager.UnloadSceneAsync(scene);
                if (operation == null) continue;

                operationGroup.Operations.Add(operation);

                OnSceneUnloaded.Invoke(scene);
            }

            foreach (var handle in handleGroup.Handles)
                if (handle.IsValid())
                    await Addressables.UnloadSceneAsync(handle);

            handleGroup.Handles.Clear();

            while (!operationGroup.IsDone) await UniTask.Delay(100);

            await Resources.UnloadUnusedAssets().ToUniTask();
        }
    }
}

public readonly struct AsyncOperationGroup
{
    public readonly List<AsyncOperation> Operations;

    public float Progress => Operations.Count == 0
        ? 0f : Operations.AsValueEnumerable().Average(o => o.progress);
    public bool IsDone => Operations.AsValueEnumerable().All(o => o.isDone);

    public AsyncOperationGroup(int initialCapacity)
        => Operations = new List<AsyncOperation>(initialCapacity);
}

public readonly struct AsyncOperationHandleGroup
{
    public readonly List<AsyncOperationHandle<SceneInstance>> Handles;

    public float Progress => Handles.Count == 0
        ? 0f : Handles.AsValueEnumerable().Average(h => h.PercentComplete);
    public bool IsDone => Handles.Count == 0 || Handles.AsValueEnumerable().All(o => o.IsDone);

    public AsyncOperationHandleGroup(int initialCapacity)
        => Handles = new List<AsyncOperationHandle<SceneInstance>>(initialCapacity);
}