using System.Collections;
using System.Collections.Generic;
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
        SoundManager soundManager;
        Coroutine playingCoroutine;

        [field: SerializeField] public AudioSource AudioSource { get; set; }
        public SoundData Data { get; private set; }
        public LinkedListNode<SoundEmitter> Node { get; set; }

        public void Initialize(SoundData data)
        {
            soundManager = IServiceLocator.Default.GetService<SoundManager>();

            Debug.Assert(data != null, "Sound emitter data is null.", this);
            Debug.Assert(data.settings != null, $"{data.name} settings is null.", data);
            Data = data;

            var clip = data.GetClip();
            Debug.Assert(clip != null, $"{data.name} clip is null.", this);

            var settings = data.settings;
            AudioSource = gameObject.GetOrAdd<AudioSource>();
            Debug.Assert(AudioSource != null, "AudioSource data is null.", this);

            AudioSource.clip = data.GetClip();
            AudioSource.volume = data.volume;
            AudioSource.pitch = data.pitch;

            AudioSource.playOnAwake = false;

            AudioSource.outputAudioMixerGroup = settings.mixerGroup;
            AudioSource.loop = settings.loop;

            AudioSource.mute = settings.mute;
            AudioSource.bypassEffects = settings.bypassEffects;
            AudioSource.bypassListenerEffects = settings.bypassListenerEffects;
            AudioSource.bypassReverbZones = settings.bypassReverbZones;

            AudioSource.priority = settings.priority;
            AudioSource.panStereo = settings.panStereo;
            AudioSource.spatialBlend = settings.spatialBlend;
            AudioSource.reverbZoneMix = settings.reverbZoneMix;
            AudioSource.dopplerLevel = settings.dopplerLevel;
            AudioSource.spread = settings.spread;

            AudioSource.minDistance = settings.minDistance;
            AudioSource.maxDistance = settings.maxDistance;

            AudioSource.ignoreListenerVolume = settings.ignoreListenerVolume;
            AudioSource.ignoreListenerPause = settings.ignoreListenerPause;

            AudioSource.rolloffMode = settings.rolloffMode;

            if (settings.rolloffMode != AudioRolloffMode.Custom) return;

            if (settings.customRolloffCurve is { length: > 0 })
                AudioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, settings.customRolloffCurve);

            if (settings.spatialBlendCurve is { length: > 0 })
                AudioSource.SetCustomCurve(AudioSourceCurveType.SpatialBlend, settings.spatialBlendCurve);

            if (settings.reverbZoneMixCurve is { length: > 0 })
                AudioSource.SetCustomCurve(AudioSourceCurveType.ReverbZoneMix, settings.reverbZoneMixCurve);

            if (settings.spreadCurve is { length: > 0 })
                AudioSource.SetCustomCurve(AudioSourceCurveType.Spread, settings.spreadCurve);
        }

        public void Play()
        {
            if (playingCoroutine != null) StopCoroutine(playingCoroutine);

            AudioSource.Play();
            playingCoroutine = StartCoroutine(WaitForSoundToEnd());
        }

        IEnumerator WaitForSoundToEnd()
        {
            yield return new WaitWhile(() => AudioSource.isPlaying);
            Stop();
        }

        public void Stop()
        {
            if (playingCoroutine != null)
            {
                StopCoroutine(playingCoroutine);
                playingCoroutine = null;
            }

            AudioSource.Stop();
            soundManager.ReturnToPool(this);
        }

        public void WithRandomPitch(float min = -0.05f, float max = 0.05f) =>
            AudioSource.pitch += Random.Range(min, max);
    }
}