using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using ElusiveWorld.Core.Assets.Scripts.Systems.Audio.Data;
using ElusiveWorld.Core.Assets.Scripts.Systems.Audio.Managers;
using ElusiveWorld.Core.Assets.Scripts.Utils.Extensions;
using ElusiveWorld.Core.Assets.Scripts.Utils.Services;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Audio.Components
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundEmitter : MonoBehaviour
    {
        [SerializeField] AudioSource audioSource;
        CancellationTokenSource playCTS;
        SoundManager soundManager;

        public SoundData Data { get; private set; }
        public LinkedListNode<SoundEmitter> Node { get; set; }

        void Awake()
        {
            audioSource = gameObject.GetOrAdd<AudioSource>();
            soundManager = IServiceLocator.Default.GetService<SoundManager>();
        }

        public void Initialize(SoundData data)
        {
            Data = data;

            audioSource.clip = data.clip;
            audioSource.outputAudioMixerGroup = data.mixerGroup;
            audioSource.loop = data.loop;
            audioSource.playOnAwake = data.playOnAwake;

            audioSource.mute = data.mute;
            audioSource.bypassEffects = data.bypassEffects;
            audioSource.bypassListenerEffects = data.bypassListenerEffects;
            audioSource.bypassReverbZones = data.bypassReverbZones;

            audioSource.priority = data.priority;
            audioSource.volume = data.volume;
            audioSource.pitch = data.pitch;
            audioSource.panStereo = data.panStereo;
            audioSource.spatialBlend = data.spatialBlend;
            audioSource.reverbZoneMix = data.reverbZoneMix;
            audioSource.dopplerLevel = data.dopplerLevel;
            audioSource.spread = data.spread;

            audioSource.minDistance = data.minDistance;
            audioSource.maxDistance = data.maxDistance;

            audioSource.ignoreListenerVolume = data.ignoreListenerVolume;
            audioSource.ignoreListenerPause = data.ignoreListenerPause;

            audioSource.rolloffMode = data.rolloffMode;
        }

        public void Play()
        {
            if (playCTS != null)
            {
                playCTS.Cancel();
                playCTS.Dispose();
            }

            playCTS = new CancellationTokenSource();
            audioSource.Play();
            WaitForSoundToEnd().Forget();
        }

        async UniTaskVoid WaitForSoundToEnd()
        {
            try
            {
                var combinedToken = playCTS.Token.CombineWithDestroyToken(this);
                await UniTask.WaitWhile(() => audioSource.isPlaying, cancellationToken: combinedToken);

                Stop();
            }
            catch (OperationCanceledException)
            {
                if (playCTS != null && !playCTS.IsCancellationRequested)
                    Cleanup();
            }
        }

        public void Stop()
        {
            if (playCTS != null)
            {
                playCTS.Cancel();
                playCTS.Dispose();
                playCTS = null;
            }

            audioSource.Stop();

            if (this != null && gameObject != null)
                soundManager.ReturnToPool(this);
        }

        void Cleanup()
        {
            playCTS?.Dispose();
            playCTS = null;
        }

        public void WithRandomPitch(float min = -0.05f, float max = 0.05f) =>
            audioSource.pitch += Random.Range(min, max);

        void OnDestroy() => Cleanup();
    }
}