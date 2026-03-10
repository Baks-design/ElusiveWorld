using Cysharp.Threading.Tasks;
using ElusiveWorld.Core.Assets.Scripts.Behaviours.Player.Movement;
using ElusiveWorld.Core.Assets.Scripts.Systems.Audio.Managers;
using ElusiveWorld.Core.Assets.Scripts.Systems.Input;
using ElusiveWorld.Core.Assets.Scripts.Systems.Tendency;
using ElusiveWorld.Core.Assets.Scripts.Systems.Weapons.Projectiles;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Game
{
    public class GameInitiator : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] EventSystem eventSystem;
        [SerializeField] CinemachineBrain cinemachineBrain;
        [Header("Systems")]
        [SerializeField] MusicManager music;
        [SerializeField] SoundManager sound;
        [SerializeField] InputManager input;
        [SerializeField] ProjectilePoolSpawner projectilePool;
        [SerializeField] ProjectileDecalPoolSpawner projectileDecalPool;
        [SerializeField] TendencyManager tendency;
        [Header("Objects")]
        [SerializeField] PlayerController player;
        [SerializeField] LoadingScreen loadingScreen;

        async void Start()
        {
            BindComponents();
            BindSystems();
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

        void BindComponents()
        {
            eventSystem = Instantiate(eventSystem);
            cinemachineBrain = Instantiate(cinemachineBrain);
        }

        void BindSystems()
        {
            input = Instantiate(input);
            sound = Instantiate(sound);
            music = Instantiate(music);
            tendency = Instantiate(tendency);
            projectilePool = Instantiate(projectilePool);
            projectileDecalPool = Instantiate(projectileDecalPool);
        }

        async UniTask InitializeSystems()
        {
            input.Initialize();
            sound.Initialize();
            music.Initialize();
            tendency.Initialize();
            await SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
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

        void OnDestroy()
        {
            input.Dispose();
            sound.Dispose();
            music.Dispose();
            tendency.Dispose();
            projectilePool.Dispose();
            projectileDecalPool.Dispose();
            player.Dispose();
        }
    }
}
