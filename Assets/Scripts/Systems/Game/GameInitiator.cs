#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using ElusiveWorld.Core.Assets.Scripts.Behaviours.Player.Movement;
using ElusiveWorld.Core.Assets.Scripts.Behaviours.Projectiles;
using ElusiveWorld.Core.Assets.Scripts.Systems.Audio.Managers;
using ElusiveWorld.Core.Assets.Scripts.Systems.Input;
using ElusiveWorld.Core.Assets.Scripts.Systems.Persistence;
using ElusiveWorld.Core.Assets.Scripts.Systems.SceneManagement;
using ElusiveWorld.Core.Assets.Scripts.Systems.Tendency;
using UnityEngine;
using UnityEngine.EventSystems;
using Unity.Cinemachine;
using Cysharp.Threading.Tasks;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Services;

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
        [SerializeField] ProjectilePoolSpawner projectilePool;
        [SerializeField] ProjectileDecalPoolSpawner projectileDecalPool;
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
            await CreateObjects();
            await InitializeObjects();
            await PrepareGame();
            await BeginGame();
        }

        void OnDestroy()
        {
            UnregisterServices();
            Dispose();
        }

        void BindComponents()
        {
            eventSystem = Instantiate(eventSystem);
            DontDestroyOnLoad(eventSystem);
            cinemachineBrain = Instantiate(cinemachineBrain);
            DontDestroyOnLoad(cinemachineBrain);
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
            projectilePool = Instantiate(projectilePool);
            DontDestroyOnLoad(projectilePool);
            projectileDecalPool = Instantiate(projectileDecalPool);
            DontDestroyOnLoad(projectileDecalPool);
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
            IServiceLocator.Default.TryRegisterService(projectilePool);
            IServiceLocator.Default.TryRegisterService(projectileDecalPool);
            IServiceLocator.Default.TryRegisterService(loadingScreen);
            IServiceLocator.Default.TryRegisterService(sceneLoader);
            IServiceLocator.Default.TryRegisterService(persistence);
        }

        async UniTask InitializeSystems()
        {
            input.Initialize();
            sound.Initialize();
            music.Initialize();
            tendency.Initialize();
            sceneLoader.Initialize();
            await sceneLoader.LoadSceneGroup(0);
        }

        async UniTask CreateObjects()
        {
            player = Instantiate(player);
            DontDestroyOnLoad(player);
        }

        async UniTask InitializeObjects()
        {
            player.Initialize();
            projectilePool.Initialize();
            projectileDecalPool.Initialize();
        }

        async UniTask PrepareGame()
        {
            input.EnableGameplay();
            player.MoveToRandomPosition();
        }

        async UniTask BeginGame() { }

        void UnregisterServices()
        {
            IServiceLocator.Default.TryUnregisterService(input);
            IServiceLocator.Default.TryUnregisterService(sound);
            IServiceLocator.Default.TryUnregisterService(music);
            IServiceLocator.Default.TryUnregisterService(tendency);
            IServiceLocator.Default.TryUnregisterService(projectilePool);
            IServiceLocator.Default.TryUnregisterService(projectileDecalPool);
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
            projectilePool.Dispose();
            projectileDecalPool.Dispose();
            player.Dispose();
            sceneLoader.Dispose();
            persistence.Dispose();
        }
    }
}