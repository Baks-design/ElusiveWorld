using UnityEngine;
using LitMotion;
using LitMotion.Extensions;
using Assets.Scripts.Internal.Runtime.Core.App.Input;
using Unity.Cinemachine;

namespace Assets.Scripts.Internal.Runtime.Core.Systems.Weapons
{
    public class WeaponGraphics : WeaponComponent<WeaponGraphics>
    {
        [SerializeField] InputReader inputReader;
        [SerializeField] Vector2 smoothAmount = new(30f, 30f);
        [SerializeField, MinMaxRangeSlider(-90f, 90f)] Vector2 minMaxYawRotationAngle = new(-30f, 30f);
        [SerializeField, MinMaxRangeSlider(-90f, 90f)] Vector2 minMaxPitchRotationAngle = new(-30f, 30f);
        [SerializeField] float smoothTime = 10f;
        MotionHandle reloadMotion;
        MotionHandle shootRotationMotion;
        MotionHandle shootPositionMotion;
        float desiredYaw;
        float desiredPitch;

        void OnEnable()
        {
            Weapon.OnWeaponReloadPressed += OnWeaponReloadPressed;
            Weapon.OnWeaponShootSucceed += OnWeaponShootSucceed;
        }

        void LateUpdate()
        {
            if (Weapon.DuringReload) return;

            desiredYaw += inputReader.LookAxis.x * smoothAmount.x * Time.deltaTime;
            desiredYaw = Mathf.Clamp(desiredYaw, minMaxYawRotationAngle.x, minMaxYawRotationAngle.y);

            desiredPitch -= inputReader.LookAxis.y * smoothAmount.y * Time.deltaTime;
            desiredPitch = Mathf.Clamp(desiredPitch, minMaxPitchRotationAngle.x, minMaxPitchRotationAngle.y);

            var targetRotation = Quaternion.Euler(desiredPitch, desiredYaw, 0f);
            var smoothFactor = 1f - Mathf.Exp(-smoothTime * Time.deltaTime);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smoothFactor);
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

            if (reloadMotion.IsActive()) reloadMotion.Cancel();

            var originalRot = transform.localEulerAngles;
            reloadMotion = LMotion.Create(originalRot, originalRot + Vector3.forward * 360f, Weapon.Data.ReloadDuration)
                .WithEase(Ease.Linear)
                .WithOnComplete(() => Weapon.OnWeaponReloadCompleted())
                .BindToLocalEulerAngles(transform);

            Weapon.OnWeaponReloadStarted();
        }

        void OnWeaponShootSucceed()
        {
            if (shootRotationMotion.IsActive()) shootRotationMotion.Cancel();
            if (shootPositionMotion.IsActive()) shootPositionMotion.Cancel();

            var originalRot = transform.localEulerAngles;
            var punchRot = Vector3.right * 30f;
            shootRotationMotion = LMotion.Create(originalRot, originalRot + punchRot, 0.2f)
                .WithEase(Ease.OutQuad)
                .WithLoops(2, LoopType.Yoyo)
                .BindToLocalEulerAngles(transform);

            var originalPos = transform.localPosition;
            var punchPos = Vector3.back;
            shootPositionMotion = LMotion.Create(originalPos, originalPos + punchPos, 0.05f)
                .WithEase(Ease.OutQuad)
                .WithLoops(2, LoopType.Yoyo)
                .BindToLocalPosition(transform);
        }
    }
}