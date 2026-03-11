#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using ElusiveWorld.Core.Assets.Scripts.Behaviours.Player.Movement;
using ElusiveWorld.Core.Assets.Scripts.Behaviours.Projectiles;
using ElusiveWorld.Core.Assets.Scripts.Graphics;
using ElusiveWorld.Core.Assets.Scripts.Systems.Audio.Managers;
using ElusiveWorld.Core.Assets.Scripts.Systems.Input;
using ElusiveWorld.Core.Assets.Scripts.Systems.Persistence;
using ElusiveWorld.Core.Assets.Scripts.Systems.SceneManagement;
using ElusiveWorld.Core.Assets.Scripts.Systems.Tendency;
using ElusiveWorld.Core.Assets.Scripts.Utils.Services;
using UnityEngine;
using UnityEngine.EventSystems;
using Unity.Cinemachine;
using Cysharp.Threading.Tasks;

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
        [SerializeField] PostProcessingManager postProcessing;
        [SerializeField] SceneLoader scenes;
        [SerializeField] PersistenceManager persistence;
        static readonly int sceneIndex = 0;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Init()
        {
#if UNITY_EDITOR
            EditorSceneManager.playModeStartScene =
                AssetDatabase.LoadAssetAtPath<SceneAsset>(EditorBuildSettings.scenes[sceneIndex].path);
#endif
        }

        async void Start()
        {
            BindComponents();
            BindSystems();
            RegisterServices();
            loadingScreen.Show();

            using var loadingScreenDisposable = new ShowLoadingScreenDisposable(loadingScreen);
            loadingScreenDisposable.SetLoadingBarPercent(0f);
            await InitializeSystems();
            loadingScreenDisposable.SetLoadingBarPercent(0.25f);
            await CreateObjects();
            loadingScreenDisposable.SetLoadingBarPercent(0.50f);
            await InitializeObjects();
            loadingScreenDisposable.SetLoadingBarPercent(0.75f);
            await PrepareGame();
            loadingScreenDisposable.SetLoadingBarPercent(1f);
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
            cinemachineBrain = Instantiate(cinemachineBrain);
            loadingScreen = Instantiate(loadingScreen);
        }

        void BindSystems()
        {
            input = Instantiate(input);
            sound = Instantiate(sound);
            music = Instantiate(music);
            tendency = Instantiate(tendency);
            projectilePool = Instantiate(projectilePool);
            projectileDecalPool = Instantiate(projectileDecalPool);
            postProcessing = Instantiate(postProcessing);
            scenes = Instantiate(scenes);
            persistence = Instantiate(persistence);
        }

        void RegisterServices()
        {
            IServiceLocator.Default.TryRegisterService(input);
            IServiceLocator.Default.TryRegisterService(sound);
            IServiceLocator.Default.TryRegisterService(music);
            IServiceLocator.Default.TryRegisterService(tendency);
            IServiceLocator.Default.TryRegisterService(projectilePool);
            IServiceLocator.Default.TryRegisterService(projectileDecalPool);
            IServiceLocator.Default.TryRegisterService(postProcessing);
            IServiceLocator.Default.TryRegisterService(scenes);
            IServiceLocator.Default.TryRegisterService(persistence);
        }

        async UniTask InitializeSystems()
        {
            input.Initialize();
            sound.Initialize();
            music.Initialize();
            tendency.Initialize();
            postProcessing.Initialize();
            await scenes.LoadSceneGroup(0);
        }

        async UniTask CreateObjects() => player = Instantiate(player);

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
            IServiceLocator.Default.TryUnregisterService(postProcessing);
            IServiceLocator.Default.TryUnregisterService(scenes);
            IServiceLocator.Default.TryUnregisterService(persistence);
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
            postProcessing.Dispose();
            scenes.Dispose();
            persistence.Dispose();
        }
    }
}
