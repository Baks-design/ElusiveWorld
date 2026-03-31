#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using ElusiveWorld.Core.Assets.Scripts.Behaviours.Player.Movement;
using ElusiveWorld.Core.Assets.Scripts.Systems.Audio.Managers;
using ElusiveWorld.Core.Assets.Scripts.Systems.Input;
using ElusiveWorld.Core.Assets.Scripts.Systems.Persistence;
using ElusiveWorld.Core.Assets.Scripts.Systems.SceneManagement;
using ElusiveWorld.Core.Assets.Scripts.Systems.Tendency;
using UnityEngine;
using UnityEngine.EventSystems;
using Unity.Cinemachine;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Services;
using ElusiveWorld.Core.Assets.Scripts.Systems.UI;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Game
{
    public class GameInitiator : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] EventSystem eventSystem;
        [SerializeField] CinemachineBrain cinemachineBrain;
        [SerializeField] LoadingScreen loadingScreen;
        [SerializeField] PlayerController player;
        [Header("Systems")]
        [SerializeField] MusicManager music;
        [SerializeField] SoundManager sound;
        [SerializeField] InputManager input;
        [SerializeField] TendencyManager tendency;
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
            BindComponents();
            BindSystems();
            RegisterServices();
            await InitializeSystems();
            CreateObjects();
            InitializeObjects();
            PrepareGame();
            BeginGame();
        }

        void OnDestroy()
        {
            UnregisterServices();
            Dispose();
        }

        void BindComponents()
        {
            cinemachineBrain = Instantiate(cinemachineBrain);
            DontDestroyOnLoad(cinemachineBrain);
            eventSystem = Instantiate(eventSystem);
            DontDestroyOnLoad(eventSystem);
            loadingScreen = Instantiate(loadingScreen);
            DontDestroyOnLoad(loadingScreen);
        }

        void BindSystems()
        {
            input = Instantiate(input);
            DontDestroyOnLoad(input);
            sound = Instantiate(sound);
            DontDestroyOnLoad(sound);
            music = Instantiate(music);
            DontDestroyOnLoad(music);
            tendency = Instantiate(tendency);
            DontDestroyOnLoad(tendency);
            sceneLoader = Instantiate(sceneLoader);
            DontDestroyOnLoad(sceneLoader);
            persistence = Instantiate(persistence);
            DontDestroyOnLoad(persistence);
        }

        void RegisterServices()
        {
            IServiceLocator.Default.TryRegisterService(input);
            IServiceLocator.Default.TryRegisterService(sound);
            IServiceLocator.Default.TryRegisterService(music);
            IServiceLocator.Default.TryRegisterService(tendency);
            IServiceLocator.Default.TryRegisterService(loadingScreen);
            IServiceLocator.Default.TryRegisterService(sceneLoader);
            IServiceLocator.Default.TryRegisterService(persistence);
        }

        async Awaitable InitializeSystems()
        {
            input.Initialize();
            sound.Initialize();
            music.Initialize();
            tendency.Initialize();
            sceneLoader.Initialize();
            await sceneLoader.LoadSceneGroup(0);
        }

        void CreateObjects()
        {
            player = Instantiate(player);
            DontDestroyOnLoad(player);
        }

        void InitializeObjects()
        {
            player.Initialize();
        }

        void PrepareGame()
        {
            input.EnableGameplay();
            player.MoveToRandomPosition();
        }

        void BeginGame() { }

        void UnregisterServices()
        {
            IServiceLocator.Default.TryUnregisterService(input);
            IServiceLocator.Default.TryUnregisterService(sound);
            IServiceLocator.Default.TryUnregisterService(music);
            IServiceLocator.Default.TryUnregisterService(tendency);
            IServiceLocator.Default.TryUnregisterService(sceneLoader);
            IServiceLocator.Default.TryUnregisterService(persistence);
            IServiceLocator.Default.TryUnregisterService(loadingScreen);
        }

        void Dispose()
        {
            input.Dispose();
            sound.Dispose();
            music.Dispose();
            tendency.Dispose();
            player.Dispose();
            sceneLoader.Dispose();
            persistence.Dispose();
        }
    }
}