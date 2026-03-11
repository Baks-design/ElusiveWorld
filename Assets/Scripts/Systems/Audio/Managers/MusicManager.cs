using System.Collections.Generic;
using ElusiveWorld.Core.Assets.Scripts.Utils.Extensions;
using ElusiveWorld.Core.Assets.Scripts.Utils.Services;
using UnityEngine;
using UnityEngine.Audio;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Audio.Managers
{
    public class MusicManager : MonoBehaviour, IService
    {
        [SerializeField] AudioMixerGroup musicMixerGroup;
        [SerializeField] List<AudioClip> initialPlaylist;
        [SerializeField] bool loopPlaylist = true;
        [SerializeField] bool loopCurrentTrack = false;
        readonly Queue<AudioClip> playlist = new();
        List<AudioClip> originalPlaylist;
        AudioSource current;
        AudioSource previous;
        const float crossFadeTime = 1f;
        float fading;

        public void Initialize()
        {
            originalPlaylist = new List<AudioClip>(initialPlaylist);
            foreach (var clip in initialPlaylist)
                AddToPlaylist(clip);
        }

        void LateUpdate()
        {
            HandleCrossFade();

            if (current && !current.isPlaying && playlist.Count > 0)
                PlayNextTrack();
        }

        public void Dispose() { }

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
                Play(nextTrack, loopCurrentTrack);
            else if (loopPlaylist && originalPlaylist.Count > 0)
            {
                foreach (var clip in originalPlaylist)
                    playlist.Enqueue(clip);

                if (playlist.TryDequeue(out var newTrack))
                    Play(newTrack, loopCurrentTrack);
            }
        }

        public void Play(AudioClip clip, bool loop = false)
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
            current.loop = loop;
            current.volume = 0f;
            current.bypassListenerEffects = true;
            current.Play();

            fading = 0.001f;
        }
    }
}