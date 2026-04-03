#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using UnityEngine.EventSystems;
using Unity.Cinemachine;
using Cysharp.Threading.Tasks;
using ElusiveWorld.Core.Assets.Scripts.Systems.Audio.Managers;
using ElusiveWorld.Core.Assets.Scripts.Systems.Input;
using ElusiveWorld.Core.Assets.Scripts.Systems.Persistence;
using ElusiveWorld.Core.Assets.Scripts.Systems.SceneManagement;
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

        async UniTask BindComponents()
        {
            DontDestroyOnLoad(this);
            cinemachineBrain = Instantiate(cinemachineBrain);
            DontDestroyOnLoad(cinemachineBrain);
            eventSystem = Instantiate(eventSystem);
            DontDestroyOnLoad(eventSystem);
            loadingScreen = Instantiate(loadingScreen);
            DontDestroyOnLoad(loadingScreen);
        }

        async UniTask BindSystems()
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

        async UniTask RegisterServices()
        {
            IServiceLocator.Default.TryRegisterService(input);
            IServiceLocator.Default.TryRegisterService(sound);
            IServiceLocator.Default.TryRegisterService(music);
            IServiceLocator.Default.TryRegisterService(loadingScreen);
            IServiceLocator.Default.TryRegisterService(sceneLoader);
            IServiceLocator.Default.TryRegisterService(persistence);
        }

        async UniTask InitializeSystems()
        {
            input.Initialize();
            sound.Initialize();
            music.Initialize();
            sceneLoader.Initialize();
            await sceneLoader.LoadSceneGroup(0);
        }

        async UniTask CreateObjects() { }

        async UniTask InitializeObjects() { }

        async UniTask PrepareGame() => input.EnableGameplay();

        void BeginGame() { }

        void UnregisterServices()
        {
            IServiceLocator.Default.TryUnregisterService(input);
            IServiceLocator.Default.TryUnregisterService(sound);
            IServiceLocator.Default.TryUnregisterService(music);
            IServiceLocator.Default.TryUnregisterService(sceneLoader);
            IServiceLocator.Default.TryUnregisterService(persistence);
            IServiceLocator.Default.TryUnregisterService(loadingScreen);
        }

        void Dispose()
        {
            input.Dispose();
            music.Dispose();
            sceneLoader.Dispose();
            persistence.Dispose();
        }
    }
}