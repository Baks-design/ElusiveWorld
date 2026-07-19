using System;
using ElusiveWorld.Core.Assets.Scripts.Systems.Audio.Components;
using ElusiveWorld.Core.Assets.Scripts.Systems.Audio.Managers;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Services;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Updates.Variable;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Updates.Variable.Interfaces;
using ElusiveWorld.Internal.Runtime.Systems.Physics;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Audio.Data
{
    public class CharacterSound : MonoBehaviour, IUpdate
    {
        [Header("References")]
        [SerializeField] CharacterSoundLibrary library;
        CharacterController controller;
        Rigidbody rigidbody;
        [Header("Settings")]
        [SerializeField] RaycastConfig raycastConfig;
        [SerializeField, Range(0.1f, 1f)] float groundCheck = 0.1f;
        [SerializeField, Range(0.1f, 1f)] float footstepInterval = 0.5f;
        [SerializeField, Range(1f, 2f)] float minFallDistance = 2f;
        PhysicsService physicsService;
        SoundManager sound;
        SoundBuilder builder;
        Func<bool> isMovingCheck;
        bool isGrounded;
        bool wasInAir;
        bool hasCharacterController;
        bool hasRigidbody;
        float footstepTimer;
        float lastYPosition;

        void OnEnable() => UpdateManager.RegisterUpdate(this);

        void Start()
        {
            controller = GetComponentInParent<CharacterController>();
            hasCharacterController = controller != null;
            rigidbody = GetComponentInParent<Rigidbody>();
            hasRigidbody = rigidbody != null;
            if (hasCharacterController)
            {
                isMovingCheck = () => controller.velocity.magnitude > 0.1f;
                lastYPosition = controller.transform.position.y;
            }
            else if (hasRigidbody)
            {
                isMovingCheck = () => rigidbody.linearVelocity.magnitude > 0.1f;
                lastYPosition = rigidbody.transform.position.y;
            }
            else
                isMovingCheck = () => true;

            raycastConfig ??= new RaycastConfig();
            physicsService = controller.gameObject.CreatePhysicsService(raycastConfig);
            if (physicsService == null)
            {
                Debug.LogError("Failed to create physics service. " +
                    "Ensure GameObject has Rigidbody or CharacterController component.");
                enabled = false;
            }

            sound = IServiceLocator.Default.GetService<SoundManager>();
            builder = sound.CreateSoundBuilder();
        }

        void IUpdate.Update()
        {
            FootstepsHandle();
            FallHandle();
        }

        void FootstepsHandle()
        {
            var (groundType, groundSetter) = DetectGroundType();
            isGrounded = groundSetter != null;

            if (isGrounded && IsMoving())
            {
                footstepTimer -= Time.deltaTime;
                if (footstepTimer <= 0f)
                {
                    PlayFootstepSound(groundType);
                    footstepTimer = footstepInterval;
                }
            }
            else footstepTimer = 0f;
        }

        void FallHandle()
        {
            var currentY = transform.position.y;
            var isCurrentlyGrounded = physicsService.Raycast.CheckGround(groundCheck).Hit;

            if (isCurrentlyGrounded && wasInAir)
            {
                var fallDistance = lastYPosition - currentY;
                if (fallDistance >= minFallDistance)
                {
                    var (groundType, _) = DetectGroundType();
                    PlayFallSound(groundType, fallDistance);
                }
            }

            if (!isCurrentlyGrounded)
            {
                lastYPosition = Mathf.Min(lastYPosition, currentY);
                wasInAir = true;
            }
            else
            {
                lastYPosition = currentY;
                wasInAir = false;
            }
        }

        bool IsMoving() => isMovingCheck();

        void PlayFootstepSound(GroundType groundType)
        {
            switch (groundType)
            {
                case GroundType.Concrete:
                    builder
                        .WithPosition(transform.position)
                        .WithRandomPitch()
                        .Play(library.ConcreteSounds);
                    break;
                case GroundType.Muddy:
                    builder
                        .WithPosition(transform.position)
                        .WithRandomPitch()
                        .Play(library.MuddySounds);
                    break;
            }
        }

        void PlayFallSound(GroundType groundType, float fallDistance)
        {
            var normalizedFall = Mathf.Clamp01(fallDistance * 0.1f);

            switch (groundType)
            {
                case GroundType.Concrete:
                    builder
                        .WithPosition(controller.transform.position)
                        .WithVolume(normalizedFall)
                        .Play(library.ConcreteSounds);
                    break;
                case GroundType.Muddy:
                    builder
                        .WithPosition(controller.transform.position)
                        .WithVolume(normalizedFall * 0.7f)
                        .Play(library.MuddySounds);
                    break;
            }
        }

        (GroundType, GroundTypeSetter) DetectGroundType()
        {
            GroundType type = default;
            GroundTypeSetter get = default;

            var raycast = physicsService.Raycast.CheckGround(groundCheck);
            if (raycast.Hit && raycast.Collider.TryGetComponent(out get))
                type = get.GroundType;

            return (type, get);
        }

        void OnDisable() => UpdateManager.UnregisterUpdate(this);
    }
}