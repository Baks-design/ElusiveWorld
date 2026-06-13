#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using Unity.Cinemachine;
using ElusiveWorld.Core.Assets.Scripts.Systems.Audio.Managers;
using ElusiveWorld.Core.Assets.Scripts.Systems.Input;
using ElusiveWorld.Core.Assets.Scripts.Systems.Persistence;
using ElusiveWorld.Core.Assets.Scripts.Systems.SceneManagement;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Services;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Game
{
    public class GameInitiator : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] CharacterController characters;
        [SerializeField] CinemachineBrain cinemachineBrain;
        [Header("Systems")]
        [SerializeField] MusicManager music;
        [SerializeField] SoundManager sound;
        [SerializeField] InputManager input;
        [SerializeField] SceneLoader sceneLoader;
        [SerializeField] PersistenceManager persistence;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Init()
        {
#if UNITY_EDITOR
            EditorSceneManager.playModeStartScene =
                AssetDatabase.LoadAssetAtPath<SceneAsset>(EditorBuildSettings.scenes[0].path);
#endif
        }

        async void Start()
        {
            await BindComponents();
            _ = BindSystems();
            _ = RegisterServices();
            await InitializeSystems();
            await CreateObjects();
            _ = InitializeObjects();
            _ = PrepareGame();
            BeginGame();
        }

        void OnDestroy()
        {
            UnregisterServices();
            Dispose();
        }

        async Awaitable BindComponents()
        {
            DontDestroyOnLoad(this);
            var brain = await InstantiateAsync(cinemachineBrain);
            DontDestroyOnLoad(brain[0]);
        }

        async Awaitable BindSystems()
        {
            input = Instantiate(input);
            DontDestroyOnLoad(input);
            sound = Instantiate(sound);
            DontDestroyOnLoad(sound);
            music = Instantiate(music);
            DontDestroyOnLoad(music);
            sceneLoader = Instantiate(sceneLoader);
            DontDestroyOnLoad(sceneLoader);
            persistence = Instantiate(persistence);
            DontDestroyOnLoad(persistence);
        }

        async Awaitable RegisterServices()
        {
            _ = IServiceLocator.Default.TryRegisterService(input);
            _ = IServiceLocator.Default.TryRegisterService(sound);
            _ = IServiceLocator.Default.TryRegisterService(music);
            _ = IServiceLocator.Default.TryRegisterService(sceneLoader);
            _ = IServiceLocator.Default.TryRegisterService(persistence);
        }

        async Awaitable InitializeSystems()
        {
            input.Initialize();
            sound.Initialize();
            music.Initialize();
            await sceneLoader.LoadSceneGroup(0);
        }

        async Awaitable CreateObjects()
        {
            var chara = await InstantiateAsync(characters);
            DontDestroyOnLoad(chara[0]);
        }

        async Awaitable InitializeObjects() { }

        async Awaitable PrepareGame() => input.EnableGameplay();

        void BeginGame() { }

        void UnregisterServices()
        {
            _ = IServiceLocator.Default.TryUnregisterService(input);
            _ = IServiceLocator.Default.TryUnregisterService(sound);
            _ = IServiceLocator.Default.TryUnregisterService(music);
            _ = IServiceLocator.Default.TryUnregisterService(sceneLoader);
            _ = IServiceLocator.Default.TryUnregisterService(persistence);
        }

        void Dispose()
        {
            input.Dispose();
            music.Dispose();
            persistence.Dispose();
        }
    }
}