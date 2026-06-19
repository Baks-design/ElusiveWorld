using System.Collections.Generic;
using ElusiveWorld.Core.Assets.Scripts.Systems.Audio.Components;
using ElusiveWorld.Core.Assets.Scripts.Systems.Audio.Data;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Services;
using UnityEngine;
using UnityEngine.Pool;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Audio.Managers
{
    public class SoundManager : MonoBehaviour, IService
    {
        [SerializeField] SoundEmitter soundEmitterPrefab;
        [SerializeField] bool collectionCheck = true;
        [SerializeField] int defaultCapacity = 10;
        [SerializeField] int maxPoolSize = 100;
        [SerializeField] int maxSoundInstances = 30;
        IObjectPool<SoundEmitter> soundEmitterPool;
        readonly List<SoundEmitter> activeSoundEmitters = new();
        public readonly LinkedList<SoundEmitter> FrequentSoundEmitters = new();

        public void Initialize()
        {
            soundEmitterPool = new ObjectPool<SoundEmitter>(
                CreateSoundEmitter, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject,
                collectionCheck, defaultCapacity, maxPoolSize);

            PreWarming();

            Debug.Log($"CountInactive: {soundEmitterPool.CountInactive}");
        }

        void PreWarming() //?
        {
            var temp = new SoundEmitter[defaultCapacity];
            for (var i = 0; i < defaultCapacity; i++) temp[i] = soundEmitterPool.Get();
            for (var i = 0; i < defaultCapacity; i++) soundEmitterPool.Release(temp[i]);
        }

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
                FrequentSoundEmitters.Remove(soundEmitter.Node);
                soundEmitter.Node = null;
            }
            soundEmitter.gameObject.SetActive(false);
            activeSoundEmitters.Remove(soundEmitter);
        }

        void OnDestroyPoolObject(SoundEmitter soundEmitter) => Destroy(soundEmitter.gameObject);

        public SoundBuilder CreateSoundBuilder() => new(this);

        public SoundEmitter Get() => soundEmitterPool.Get();

        public bool CanPlaySound(SoundData data)
        {
            if (!data.frequentSound ||
                FrequentSoundEmitters.Count < maxSoundInstances)
                return true;

            try
            {
                FrequentSoundEmitters.First.Value.Stop();
                return true;
            }
            catch { Debug.Log("SoundEmitter is already released"); }

            return false;
        }

        public void StopAll()
        {
            var tempList = new LinkedList<SoundEmitter>(activeSoundEmitters);
            foreach (var soundEmitter in tempList) soundEmitter.Stop();
            FrequentSoundEmitters.Clear();
        }

        public void ReturnToPool(SoundEmitter soundEmitter) =>
            soundEmitterPool.Release(soundEmitter);
    }
}