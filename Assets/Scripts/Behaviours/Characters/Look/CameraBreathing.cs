using System;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Characters
{
    public class CameraBreathing
    {
        readonly CharactersLookFlags flags;
        readonly LookSettings settings;
        readonly Transform camTransform;
        readonly PerlinNoiseScroller perlinNoiseScroller;
        readonly bool affectPosition;
        readonly bool affectRotation;
        Vector3 lastPositionOffset;
        Quaternion lastRotationOffset = Quaternion.identity;

        public CameraBreathing(
            CharactersLookFlags flags,
            LookSettings settings,
            Transform camTransform)
        {
            this.flags = flags ?? throw new ArgumentNullException(nameof(flags));
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this.camTransform = camTransform != null ? camTransform : throw new ArgumentNullException(nameof(camTransform));
            if (settings.data == null) throw new ArgumentNullException(nameof(settings.data));

            perlinNoiseScroller = new PerlinNoiseScroller(settings.data);

            affectPosition =
                settings.data.transformTarget == TransformTarget.Position ||
                settings.data.transformTarget == TransformTarget.Both;
            affectRotation =
                settings.data.transformTarget == TransformTarget.Rotation ||
                settings.data.transformTarget == TransformTarget.Both;
        }

        public void Update()
        {
            if (!flags.IsCanBreathing)
            {
                ResetOffsets();
                return;
            }

            camTransform.localPosition -= lastPositionOffset;
            camTransform.localRotation *= Quaternion.Inverse(lastRotationOffset);

            perlinNoiseScroller.Update();
            var noise = perlinNoiseScroller.Noise;

            lastPositionOffset = affectPosition ? GetPositionOffset(noise) : Vector3.zero;
            lastRotationOffset = affectRotation ? GetRotationOffset(noise) : Quaternion.identity;

            camTransform.localPosition += lastPositionOffset;
            camTransform.localRotation *= lastRotationOffset;
        }

        void ResetOffsets()
        {
            camTransform.localPosition -= lastPositionOffset;
            camTransform.localRotation *= Quaternion.Inverse(lastRotationOffset);

            lastPositionOffset = Vector3.zero;
            lastRotationOffset = Quaternion.identity;
        }

        Vector3 GetPositionOffset(Vector3 noise) =>
            new(settings.x ? noise.x : 0f,
                settings.y ? noise.y : 0f,
                settings.z ? noise.z : 0f);

        Quaternion GetRotationOffset(Vector3 noise) =>
            Quaternion.Euler(
                settings.x ? noise.x : 0f,
                settings.y ? noise.y : 0f,
                settings.z ? noise.z : 0f);
    }
}