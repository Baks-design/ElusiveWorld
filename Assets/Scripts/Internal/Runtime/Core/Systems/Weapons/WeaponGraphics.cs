using UnityEngine;
using LitMotion;
using LitMotion.Extensions;

namespace Assets.Scripts.Internal.Runtime.Core.Systems.Weapons
{
    public class WeaponGraphics : WeaponComponent<WeaponGraphics>
    {
        MotionHandle reloadMotion;
        MotionHandle shootRotationMotion;
        MotionHandle shootPositionMotion;
        LineRenderer lineRenderer;

        void OnEnable()
        {
            Weapon.OnWeaponReloadPressed += OnWeaponReloadPressed;
            Weapon.OnWeaponShootSucceed += OnWeaponShootSucceed;
        }

        void Start() => lineRenderer = GetComponentInChildren<LineRenderer>();

        void LateUpdate() //FIXME: This
        {
            if (Weapon.DuringReload) return;

            var lookVector = Weapon.CurrentAimPoint - Weapon.ProjectileAnchor.transform.position;
            var desiredLookRot = Quaternion.LookRotation(lookVector.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredLookRot, Time.deltaTime * 10f);

            var lookVectorMagnitude = lookVector.magnitude;
            lineRenderer.SetPosition(1, Vector3.forward * lookVectorMagnitude);
        }
        
        void OnDisable()
        {
            Weapon.OnWeaponReloadPressed -= OnWeaponReloadPressed;
            Weapon.OnWeaponShootSucceed -= OnWeaponShootSucceed;

            if (reloadMotion.IsActive()) reloadMotion.Cancel();
            if (shootRotationMotion.IsActive()) shootRotationMotion.Cancel();
            if (shootPositionMotion.IsActive()) shootPositionMotion.Cancel();
        }

        void OnWeaponReloadPressed()
        {
            if (Weapon.DuringReload) return;

            var startRot = transform.localEulerAngles;
            var desiredRot = transform.localEulerAngles + Vector3.forward * 360f;

            // Cancel any existing reload motion
            if (reloadMotion.IsActive()) reloadMotion.Cancel();

            reloadMotion = LMotion.Create(startRot, desiredRot, Weapon.Data.ReloadDuration)
                .WithEase(Ease.Linear)
                .WithOnComplete(() => Weapon.OnWeaponReloadCompleted())
                .BindToLocalEulerAngles(transform);

            // Trigger start event immediately
            Weapon.OnWeaponReloadStarted();
        }

        void OnWeaponShootSucceed()
        {
            // Cancel any existing shoot motions
            if (shootRotationMotion.IsActive()) shootRotationMotion.Cancel();
            if (shootPositionMotion.IsActive()) shootPositionMotion.Cancel();

            // Store original values
            var originalRot = transform.localEulerAngles;
            var originalPos = transform.localPosition;

            // Punch rotation effect
            var punchRot = Vector3.right * 30f;
            shootRotationMotion = LMotion.Create(originalRot, originalRot + punchRot, 0.2f)
                .WithEase(Ease.OutQuad)
                .WithLoops(2, LoopType.Yoyo)
                .BindToLocalEulerAngles(transform);

            // Punch position effect
            var punchPos = Vector3.back;
            shootPositionMotion = LMotion.Create(originalPos, originalPos + punchPos, 0.05f)
                .WithEase(Ease.OutQuad)
                .WithLoops(2, LoopType.Yoyo)
                .BindToLocalPosition(transform);
        }

    }
}
