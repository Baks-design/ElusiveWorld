using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using ElusiveWorld.Core.Assets.Scripts.Systems.Audio.Data;
using ElusiveWorld.Core.Assets.Scripts.Systems.Audio.Managers;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Services;
using ElusiveWorld.Core.Assets.Scripts.Utils.Extensions;
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
            Debug.Assert(data != null, "Sound emitter data is null.", this);
            Debug.Assert(data.settings != null, $"{data.name} + settings is null.", data);

            var clip = data.GetClip();
            Debug.Assert(clip != null, $"{data.name} + clip is null.", this);

            audioSource.clip = data.GetClip();
            audioSource.volume = data.volume;
            audioSource.pitch = data.pitch;

            audioSource.playOnAwake = false;

            var settings = data.settings;

            audioSource.outputAudioMixerGroup = settings.mixerGroup;
            audioSource.loop = settings.loop;

            audioSource.mute = settings.mute;
            audioSource.bypassEffects = settings.bypassEffects;
            audioSource.bypassListenerEffects = settings.bypassListenerEffects;
            audioSource.bypassReverbZones = settings.bypassReverbZones;

            audioSource.priority = settings.priority;
            audioSource.panStereo = settings.panStereo;
            audioSource.spatialBlend = settings.spatialBlend;
            audioSource.reverbZoneMix = settings.reverbZoneMix;
            audioSource.dopplerLevel = settings.dopplerLevel;
            audioSource.spread = settings.spread;

            audioSource.minDistance = settings.minDistance;
            audioSource.maxDistance = settings.maxDistance;

            audioSource.ignoreListenerVolume = settings.ignoreListenerVolume;
            audioSource.ignoreListenerPause = settings.ignoreListenerPause;

            audioSource.rolloffMode = settings.rolloffMode;

            if (settings.rolloffMode != AudioRolloffMode.Custom)
                return;

            if (settings.customRolloffCurve is { length: > 0 })
                audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, settings.customRolloffCurve);

            if (settings.spatialBlendCurve is { length: > 0 })
                audioSource.SetCustomCurve(AudioSourceCurveType.SpatialBlend, settings.spatialBlendCurve);

            if (settings.reverbZoneMixCurve is { length: > 0 })
                audioSource.SetCustomCurve(AudioSourceCurveType.ReverbZoneMix, settings.reverbZoneMixCurve);

            if (settings.spreadCurve is { length: > 0 })
                audioSource.SetCustomCurve(AudioSourceCurveType.Spread, settings.spreadCurve);
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