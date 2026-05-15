using System.Collections.Generic;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Services;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Updates.Variable;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Updates.Variable.Interfaces;
using ElusiveWorld.Core.Assets.Scripts.Utils.Extensions;
using UnityEngine;
using UnityEngine.Audio;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Audio.Managers
{
    [RequireComponent(typeof(MusicManager))]
    public class MusicManager : MonoBehaviour, IUpdate, IService
    {
        [SerializeField] AudioMixerGroup musicMixerGroup;
        [SerializeField] List<AudioClip> initialPlaylist;
        AudioSource current;
        AudioSource previous;
        float fading;
        readonly Queue<AudioClip> playlist = new();
        const float crossFadeTime = 1f;

        public void Initialize()
        {
            UpdateManager.RegisterUpdate(this);
            foreach (var clip in initialPlaylist) AddToPlaylist(clip);
        }

        void IUpdate.Update()
        {
            HandleCrossFade();

            if (current && !current.isPlaying && playlist.Count > 0) PlayNextTrack();
        }

        void HandleCrossFade()
        {
            if (fading <= 0f) return;

            fading += Time.deltaTime;

            var fraction = Mathf.Clamp01(fading / crossFadeTime);
            var logFraction = fraction.ToLogarithmicFraction();

            if (previous) previous.volume = 1f - logFraction;
            if (current) current.volume = logFraction;
            if (fraction >= 1)
            {
                fading = 0.0f;
                if (previous)
                {
                    Destroy(previous);
                    previous = null;
                }
            }
        }

        public void AddToPlaylist(AudioClip clip)
        {
            playlist.Enqueue(clip);
            if (current == null && previous == null) PlayNextTrack();
        }

        public void Clear() => playlist.Clear();

        public void PlayNextTrack()
        {
            if (playlist.TryDequeue(out var nextTrack))  Play(nextTrack);
        }

        public void Play(AudioClip clip)
        {
            if (current && current.clip == clip) return;

            if (previous)
            {
                Destroy(previous);
                previous = null;
            }

            previous = current;

            current = gameObject.GetOrAdd<AudioSource>();
            current.clip = clip;
            current.outputAudioMixerGroup = musicMixerGroup;
            current.loop = false;
            current.volume = 0f;
            current.bypassListenerEffects = true;
            current.Play();

            fading = 0.001f;
        }


        public void Dispose() => UpdateManager.UnregisterUpdate(this);
    }
}