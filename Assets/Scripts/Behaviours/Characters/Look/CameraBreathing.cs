using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Characters
{
    public class CameraBreathing
    {
        readonly LookSettings settings;
        readonly Transform camTransform;
        readonly PerlinNoiseScroller perlinNoiseScroller;
        Vector3 originalPosition;
        Vector3 originalRotation;

        public bool IsCanBreathing { get; set; } = false;

        public CameraBreathing(LookSettings settings, Transform camTransform)
        {
            this.settings = settings;
            this.camTransform = camTransform;

            perlinNoiseScroller = new PerlinNoiseScroller(settings.data);

            originalPosition = camTransform.localPosition;
            originalRotation = camTransform.localEulerAngles;
        }

        public void Update()
        {
            if (!IsCanBreathing) return;

            perlinNoiseScroller.Update();

            var posOffset = GetPositionOffset();
            var rotOffset = GetRotationOffset();
            ApplyOffsets(posOffset, rotOffset);
        }

        Vector3 GetPositionOffset() => new(
            settings.x && ShouldAffectPosition() ? perlinNoiseScroller.Noise.x : 0f,
            settings.y && ShouldAffectPosition() ? perlinNoiseScroller.Noise.y : 0f,
            settings.z && ShouldAffectPosition() ? perlinNoiseScroller.Noise.z : 0f
        );

        Vector3 GetRotationOffset() => new(
            settings.x && ShouldAffectRotation() ? perlinNoiseScroller.Noise.x : 0f,
            settings.y && ShouldAffectRotation() ? perlinNoiseScroller.Noise.y : 0f,
            settings.z && ShouldAffectRotation() ? perlinNoiseScroller.Noise.z : 0f
        );

        void ApplyOffsets(Vector3 posOffset, Vector3 rotOffset)
        {
            if (ShouldAffectPosition())
                camTransform.localPosition = new Vector3(
                    settings.x ? originalPosition.x + posOffset.x : camTransform.localPosition.x,
                    settings.y ? originalPosition.y + posOffset.y : camTransform.localPosition.y,
                    settings.z ? originalPosition.z + posOffset.z : camTransform.localPosition.z
                );

            if (ShouldAffectRotation())
                camTransform.localEulerAngles = new Vector3(
                    settings.x ? originalRotation.x + rotOffset.x : camTransform.localEulerAngles.x,
                    settings.y ? originalRotation.y + rotOffset.y : camTransform.localEulerAngles.y,
                    settings.z ? originalRotation.z + rotOffset.z : camTransform.localEulerAngles.z
                );
        }

        bool ShouldAffectPosition() =>
            settings.data.transformTarget == TransformTarget.Position ||
            settings.data.transformTarget == TransformTarget.Both;

        bool ShouldAffectRotation() =>
            settings.data.transformTarget == TransformTarget.Rotation ||
            settings.data.transformTarget == TransformTarget.Both;
    }
}