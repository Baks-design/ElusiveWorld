using System;
using UnityEngine;
using Random = UnityEngine.Random;
using ElusiveWorld.Core.Assets.Scripts.Behaviours.Player.Sound.Data;
using ElusiveWorld.Core.Assets.Scripts.Systems.Audio.Components;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Player.Sound
{
    [Serializable]
    public class FootstepsSounds //FIXME: Footsteps Function
    {
        [Header("Detection Settings")]
        [SerializeField] float raycastDistance = 1.5f;
        [SerializeField] float sphereCastRadius = 0.1f;
        [SerializeField] LayerMask groundLayers = -1;
        [SerializeField] float checkInterval = 0.1f;
        [SerializeField] bool debugMode = false;
        [Header("Ground Type Settings")]
        [SerializeField] GroundTypeSettings defaultGroundType;
        [SerializeField] GroundTypeSettings[] groundTypes;
        SoundBuilder soundBuilder;
        Transform groundCheckPoint;
        float lastCheckTime;

        public GroundTypeSettings GetCurrentGroundSettings { get; private set; }
        public string CurrentGroundType { get; set; }
        public GameObject CurrentGroundObject { get; set; }
        public Vector3 GroundHitPoint { get; set; }

        public void Initialize(Transform groundCheckPoint, SoundBuilder soundBuilder)
        {
            this.groundCheckPoint = groundCheckPoint;
            this.soundBuilder = soundBuilder;

            CurrentGroundType = defaultGroundType != null ? defaultGroundType.groundTypeName : "Default";
            GetCurrentGroundSettings = defaultGroundType;
        }

        public void Update()
        {
            if (Time.time >= lastCheckTime + checkInterval)
            {
                DetectGroundType();
                lastCheckTime = Time.time;
            }
        }

        void DetectGroundType()
        {
            var rayOrigin = groundCheckPoint.position;

            if (Physics.SphereCast(
                rayOrigin, sphereCastRadius, Vector3.down, out var hit,
                raycastDistance, groundLayers, QueryTriggerInteraction.Ignore))
                UpdateGroundInfo(hit);

            if (debugMode)
                Debug.DrawRay(
                    rayOrigin, Vector3.down * raycastDistance,
                    CurrentGroundObject != null ? Color.green : Color.red);
        }

        void UpdateGroundInfo(RaycastHit hit)
        {
            GroundHitPoint = hit.point;

            if (hit.collider.gameObject != CurrentGroundObject)
            {
                CurrentGroundObject = hit.collider.gameObject;

                var newGroundType = DetermineGroundType(hit);
                if (newGroundType != CurrentGroundType)
                {
                    CurrentGroundType = newGroundType;

                    if (debugMode)
                        Debug.Log($"Ground type changed to: {CurrentGroundType}");
                }
            }
        }

        string DetermineGroundType(RaycastHit hit)
        {
            var groundObject = hit.collider.gameObject;

            foreach (var groundType in groundTypes)
                if ((groundLayers.value & (1 << groundObject.layer)) != 0)
                    return groundType.groundTypeName;

            GetCurrentGroundSettings = defaultGroundType;
            return defaultGroundType != null ? defaultGroundType.groundTypeName : "Default";
        }

        public void PlayFootstep()
        {
            if (GetCurrentGroundSettings == null || GetCurrentGroundSettings.soundData.GetClips.Length == 0)
                return;

            var clipToPlay = GetCurrentGroundSettings.soundData.GetClips[
                Random.Range(0, GetCurrentGroundSettings.soundData.GetClips.Length)];
            if (clipToPlay != null)
            {
                Debug.Log("Play");
                
                soundBuilder
                    .WithRandomPitch()
                    .WithPosition(groundCheckPoint.position)
                    .Play(clipToPlay);

                if (debugMode)
                    Debug.Log($"Footstep played on {CurrentGroundType}");
            }
        }

        public void DrawGizmos()
        {
            if (groundCheckPoint == null) return;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheckPoint.position, sphereCastRadius);
            Gizmos.DrawLine(groundCheckPoint.position, groundCheckPoint.position + Vector3.down * raycastDistance);
        }
    }
}