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
            await BindSystems();
            await RegisterServices();
            await InitializeSystems();
            await CreateObjects();
            await InitializeObjects();
            await PrepareGame();
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
            var brain = Instantiate(cinemachineBrain);
            DontDestroyOnLoad(brain);
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
            IServiceLocator.Default.TryRegisterService(input);
            IServiceLocator.Default.TryRegisterService(sound);
            IServiceLocator.Default.TryRegisterService(music);
            IServiceLocator.Default.TryRegisterService(sceneLoader);
            IServiceLocator.Default.TryRegisterService(persistence);
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
            var chara = Instantiate(characters);
            DontDestroyOnLoad(chara);
        }

        async Awaitable InitializeObjects() { }

        async Awaitable PrepareGame() => input.EnableGameplay();

        void BeginGame() { }

        void UnregisterServices()
        {
            IServiceLocator.Default.TryUnregisterService(input);
            IServiceLocator.Default.TryUnregisterService(sound);
            IServiceLocator.Default.TryUnregisterService(music);
            IServiceLocator.Default.TryUnregisterService(sceneLoader);
            IServiceLocator.Default.TryUnregisterService(persistence);
        }

        void Dispose()
        {
            input.Dispose();
            music.Dispose();
            persistence.Dispose();
        }
    }
}