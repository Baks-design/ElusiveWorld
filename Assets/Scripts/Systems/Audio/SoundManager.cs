using System.Collections.Generic;
using Assets.Scripts.Internal.Runtime.Core.Utils.Services;
using UnityEngine;
using UnityEngine.Pool;

namespace Assets.Scripts.Internal.Runtime.Core.Systems.Audio
{
    public class SoundManager : MonoBehaviour, IService
    {
        [SerializeField] SoundEmitter soundEmitterPrefab;
        [SerializeField] bool collectionCheck = false;
        [SerializeField] int defaultCapacity = 10;
        [SerializeField] int maxPoolSize = 100;
        [SerializeField] int maxSoundInstances = 30;
        IObjectPool<SoundEmitter> soundEmitterPool;
        readonly List<SoundEmitter> activeSoundEmitters = new();
        readonly LinkedList<SoundEmitter> frequentSoundEmitters = new();

        public LinkedList<SoundEmitter> FrequentSoundEmitters => frequentSoundEmitters;

        void Awake() => IServiceLocator.Default.TryRegisterService(this);

        void Start() => InitializePool();

        public SoundBuilder CreateSoundBuilder() => new(this);

        public bool CanPlaySound(SoundData data)
        {
            if (!data.frequentSound) return true;

            if (frequentSoundEmitters.Count >= maxSoundInstances)
            {
                try
                {
                    frequentSoundEmitters.First.Value.Stop();
                    return true;
                }
                catch
                {
                    Debug.Log("SoundEmitter is already released");
                }
                return false;
            }
            return true;
        }

        public SoundEmitter Get() => soundEmitterPool.Get();

        public void ReturnToPool(SoundEmitter soundEmitter) => soundEmitterPool.Release(soundEmitter);

        public void StopAll()
        {
            var tempList = new LinkedList<SoundEmitter>(activeSoundEmitters);
            foreach (var soundEmitter in tempList) soundEmitter.Stop();

            frequentSoundEmitters.Clear();
        }

        void InitializePool() =>
            soundEmitterPool = new ObjectPool<SoundEmitter>(
                CreateSoundEmitter,
                OnTakeFromPool,
                OnReturnedToPool,
                OnDestroyPoolObject,
                collectionCheck,
                defaultCapacity,
                maxPoolSize);

        SoundEmitter CreateSoundEmitter()
        {
            var soundEmitter = Instantiate(soundEmitterPrefab);
            soundEmitter.gameObject.SetActive(false);
            return soundEmitter;
        }

        void OnTakeFromPool(SoundEmitter soundEmitter)
        {
            soundEmitter.gameObject.SetActive(true);
            activeSoundEmitters.Add(soundEmitter);
        }

        void OnReturnedToPool(SoundEmitter soundEmitter)
        {
            if (soundEmitter.Node != null)
            {
                frequentSoundEmitters.Remove(soundEmitter.Node);
                soundEmitter.Node = null;
            }
            soundEmitter.gameObject.SetActive(false);
            activeSoundEmitters.Remove(soundEmitter);
        }

        void OnDestroyPoolObject(SoundEmitter soundEmitter) => Destroy(soundEmitter.gameObject);
    }
}