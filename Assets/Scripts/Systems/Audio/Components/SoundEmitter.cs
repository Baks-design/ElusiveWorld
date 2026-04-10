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
        Coroutine playingCoroutine;
        AudioSource source;
        SoundManager sound;

        public SoundData Data { get; set; }
        public LinkedListNode<SoundEmitter> Node { get; set; }

        void Awake() => source = gameObject.GetOrAdd<AudioSource>();

        void OnEnable() => sound = IServiceLocator.Default.GetService<SoundManager>();

        public void Initialize(SoundData data)
        {
            Data = data;
            Debug.Assert(data != null, "Sound emitter data is null.", this);
            Debug.Assert(data.settings != null, $"{data.name} + settings is null.", data);

            var clip = data.GetClip();
            Debug.Assert(clip != null, $"{data.name} + clip is null.", this);

            var settings = data.settings;

            source.clip = data.GetClip();
            source.volume = data.volume;
            source.pitch = data.pitch;
            source.playOnAwake = false;
            source.outputAudioMixerGroup = settings.mixerGroup;
            source.loop = settings.loop;
            source.mute = settings.mute;
            source.bypassEffects = settings.bypassEffects;
            source.bypassListenerEffects = settings.bypassListenerEffects;
            source.bypassReverbZones = settings.bypassReverbZones;
            source.priority = settings.priority;
            source.panStereo = settings.panStereo;
            source.spatialBlend = settings.spatialBlend;
            source.reverbZoneMix = settings.reverbZoneMix;
            source.dopplerLevel = settings.dopplerLevel;
            source.spread = settings.spread;
            source.minDistance = settings.minDistance;
            source.maxDistance = settings.maxDistance;
            source.ignoreListenerVolume = settings.ignoreListenerVolume;
            source.ignoreListenerPause = settings.ignoreListenerPause;
            source.rolloffMode = settings.rolloffMode;

            if (settings.rolloffMode != AudioRolloffMode.Custom) return;
            if (settings.customRolloffCurve is { length: > 0 })
                source.SetCustomCurve(AudioSourceCurveType.CustomRolloff, settings.customRolloffCurve);
            if (settings.spatialBlendCurve is { length: > 0 })
                source.SetCustomCurve(AudioSourceCurveType.SpatialBlend, settings.spatialBlendCurve);
            if (settings.reverbZoneMixCurve is { length: > 0 })
                source.SetCustomCurve(AudioSourceCurveType.ReverbZoneMix, settings.reverbZoneMixCurve);
            if (settings.spreadCurve is { length: > 0 })
                source.SetCustomCurve(AudioSourceCurveType.Spread, settings.spreadCurve);
        }

        public void Play()
        {
            if (playingCoroutine != null) StopCoroutine(playingCoroutine);
            source.Play();
            playingCoroutine = StartCoroutine(WaitForSoundToEnd());
        }

        IEnumerator WaitForSoundToEnd()
        {
            yield return new WaitWhile(() => source.isPlaying);
            Stop();
        }

        public void Stop()
        {
            if (playingCoroutine != null)
            {
                StopCoroutine(playingCoroutine);
                playingCoroutine = null;
            }
            source.Stop();
            sound.ReturnToPool(this);
        }

        public void WithRandomPitch(float min = -0.05f, float max = 0.05f)
            => source.pitch += Random.Range(min, max);
    }
}