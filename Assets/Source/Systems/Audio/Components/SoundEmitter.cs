using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        //Coroutine playingCoroutine;
        SoundManager soundManager;
        bool isPlaying;

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
            audioSource = gameObject.GetOrAdd<AudioSource>();
            Debug.Assert(audioSource != null, "AudioSource data is null.", this);

            audioSource.clip = data.GetClip();
            audioSource.volume = data.volume;
            audioSource.pitch = data.pitch;

            audioSource.playOnAwake = false;

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

            if (settings.rolloffMode != AudioRolloffMode.Custom) return;

            if (settings.customRolloffCurve is { length: > 0 })
                audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, settings.customRolloffCurve);

            if (settings.spatialBlendCurve is { length: > 0 })
                audioSource.SetCustomCurve(AudioSourceCurveType.SpatialBlend, settings.spatialBlendCurve);

            if (settings.reverbZoneMixCurve is { length: > 0 })
                audioSource.SetCustomCurve(AudioSourceCurveType.ReverbZoneMix, settings.reverbZoneMixCurve);

            if (settings.spreadCurve is { length: > 0 })
                audioSource.SetCustomCurve(AudioSourceCurveType.Spread, settings.spreadCurve);
        }

        public async Awaitable Play()
        {
            if (isPlaying) Stop();

            isPlaying = true;
            audioSource.Play();

            await AwaitableExtensions.WaitUntil(() => audioSource.isPlaying && isPlaying);

            if (isPlaying) Stop();
        }

        public void Stop()
        {
            isPlaying = false;
            audioSource.Stop();
            soundManager.ReturnToPool(this);
        }

        // public void Play()
        // {
        //     if (playingCoroutine != null) StopCoroutine(playingCoroutine);

        //     audioSource.Play();
        //     playingCoroutine = StartCoroutine(WaitForSoundToEnd());
        // }

        // IEnumerator WaitForSoundToEnd()
        // {
        //     yield return new WaitWhile(() => audioSource.isPlaying);
        //     Stop();
        // }

        // public void Stop()
        // {
        //     if (playingCoroutine != null)
        //     {
        //         StopCoroutine(playingCoroutine);
        //         playingCoroutine = null;
        //     }
        //     audioSource.Stop();
        //     soundManager.ReturnToPool(this);
        // }

        public void WithRandomPitch(float min = -0.05f, float max = 0.05f) =>
            audioSource.pitch += Random.Range(min, max);
    }
}