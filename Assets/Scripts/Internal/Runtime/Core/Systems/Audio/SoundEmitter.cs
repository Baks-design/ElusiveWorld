using System;
using System.Collections.Generic;
using System.Threading;
using Assets.Scripts.Internal.Runtime.Core.Utils.Extensions;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Internal.Runtime.Core.Systems.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundEmitter : MonoBehaviour
    {
        CancellationTokenSource playCTS;
        AudioSource audioSource;

        public SoundData Data { get; private set; }
        public LinkedListNode<SoundEmitter> Node { get; set; }

        void Awake() => audioSource = gameObject.GetOrAdd<AudioSource>();

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
                SoundManager.Instance.ReturnToPool(this);
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