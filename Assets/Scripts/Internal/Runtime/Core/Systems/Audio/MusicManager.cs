using System.Collections.Generic;
using Assets.Scripts.Internal.Runtime.Core.Utils;
using Assets.Scripts.Internal.Runtime.Core.Utils.Extensions;
using UnityEngine;
using UnityEngine.Audio;

namespace Assets.Scripts.Internal.Runtime.Core.Systems.Audio
{
    [RequireComponent(typeof(MusicManager))]
    public class MusicManager : Singleton<MonoBehaviour>
    {
        [SerializeField] AudioMixerGroup musicMixerGroup;
        [SerializeField] List<AudioClip> initialPlaylist;
        readonly Queue<AudioClip> playlist = new();
        AudioSource current;
        AudioSource previous;
        const float crossFadeTime = 1f;
        float fading;

        void Start()
        {
            foreach (var clip in initialPlaylist)
                AddToPlaylist(clip);
        }

        public void AddToPlaylist(AudioClip clip)
        {
            playlist.Enqueue(clip);
            if (current == null && previous == null)
                PlayNextTrack();
        }

        public void Clear() => playlist.Clear();

        public void PlayNextTrack()
        {
            if (playlist.TryDequeue(out var nextTrack))
                Play(nextTrack);
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
            current.outputAudioMixerGroup = musicMixerGroup; // Set mixer group
            current.loop = false; // For playlist functionality, we want tracks to play once
            current.volume = 0f;
            current.bypassListenerEffects = true;
            current.Play();

            fading = 0.001f;
        }

        void Update()
        {
            HandleCrossFade();

            if (current && !current.isPlaying && playlist.Count > 0)
                PlayNextTrack();
        }

        void HandleCrossFade()
        {
            if (fading <= 0f) return;

            fading += Time.deltaTime;

            var fraction = Mathf.Clamp01(fading / crossFadeTime);
            var logFraction = fraction.ToLogarithmicFraction();

            if (previous) previous.volume = 1f - logFraction;
            if (current) current.volume = logFraction;

            if (fraction >= 1f)
            {
                fading = 0f;
                if (previous)
                {
                    Destroy(previous);
                    previous = null;
                }
            }
        }
    }
}