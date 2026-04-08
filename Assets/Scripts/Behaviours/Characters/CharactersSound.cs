using UnityEngine;
using R3;
using ElusiveWorld.Core.Assets.Scripts.Systems.Audio.Data;
using ElusiveWorld.Core.Assets.Scripts.Systems.Audio.Managers;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Services;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Characters
{
    public class CharactersSound : MonoBehaviour
    {
        [SerializeField] CharacterController controller;
        [SerializeField] Transform spawn;
        [SerializeField] SoundLibraryObject footstepsLibrary;

        void Start() => PlayFootStepsSound();

        void PlayFootStepsSound()
            => Observable
                .EveryUpdate()
                .Where(_ => controller.velocity.magnitude > 0.1f)
                .Subscribe(_ =>
                {
                    var soundManager = IServiceLocator.Default.GetService<SoundManager>();
                    var soundBuilder = soundManager.CreateSoundBuilder();
                    soundBuilder.WithRandomPitch().WithPosition(spawn.position).Play(footstepsLibrary.GetClips[0]);
                })
                .AddTo(this);
    }
}