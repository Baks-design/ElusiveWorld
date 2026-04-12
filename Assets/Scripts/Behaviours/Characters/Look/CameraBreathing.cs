using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Characters
{
    public class CameraBreathing
    {
        readonly CharactersLookFlags flags;
        readonly LookSettings settings;
        readonly Transform camTransform;
        PerlinNoiseScroller perlinNoiseScroller;
        Vector3 originalPosition;
        Vector3 originalRotation;

        public CameraBreathing(
            CharactersLookFlags flags,
            LookSettings settings,
            Transform camTransform)
        {
            this.flags = flags;
            this.settings = settings;
            this.camTransform = camTransform;

            InitializeSettings();
        }

        public void Update()
        {
            if (!flags.IsCanBreathing) return;

            perlinNoiseScroller.Update();
            var noise = perlinNoiseScroller.Noise;

            var affectPos = ShouldAffectPosition();
            if (affectPos) ApplyPosition(noise);

            var affectRot = ShouldAffectRotation();
            if (affectRot) ApplyRotation(noise);
        }

        void InitializeSettings()
        {
            flags.IsCanBreathing = false;
            perlinNoiseScroller = new PerlinNoiseScroller(settings.data);
            originalPosition = camTransform.localPosition;
            originalRotation = camTransform.localEulerAngles;
        }

        void ApplyPosition(Vector3 noise)
        {
            var current = camTransform.localPosition;
            camTransform.localPosition = new Vector3(
                settings.x ? originalPosition.x + noise.x : current.x,
                settings.y ? originalPosition.y + noise.y : current.y,
                settings.z ? originalPosition.z + noise.z : current.z
            );
        }

        void ApplyRotation(Vector3 noise)
        {
            var current = camTransform.localEulerAngles;
            camTransform.localEulerAngles = new Vector3(
                settings.x ? originalRotation.x + noise.x : current.x,
                settings.y ? originalRotation.y + noise.y : current.y,
                settings.z ? originalRotation.z + noise.z : current.z
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