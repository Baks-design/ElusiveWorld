using UnityEngine;
using LitMotion;
using LitMotion.Extensions;
using Unity.Cinemachine;
using ElusiveWorld.Core.Assets.Scripts.Systems.Input;
using ElusiveWorld.Core.Assets.Scripts.Utils.Services;
using ElusiveWorld.Core.Assets.Scripts.Utils.Extensions;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Weapons
{
    public class WeaponGraphics : WeaponComponent<WeaponGraphics>
    {
        [SerializeField] Vector2 smoothAmount = new(30f, 30f);
        [SerializeField, MinMaxRangeSlider(-90f, 90f)] Vector2 minMaxYawRotationAngle = new(-30f, 30f);
        [SerializeField, MinMaxRangeSlider(-90f, 90f)] Vector2 minMaxPitchRotationAngle = new(-30f, 30f);
        [SerializeField] float smoothTime = 10f;
        InputManager input;
        MotionHandle reloadMotion;
        MotionHandle shootRotationMotion;
        MotionHandle shootPositionMotion;
        float desiredYaw;
        float desiredPitch;

        void OnEnable()
        {
            input = IServiceLocator.Default.GetService<InputManager>();
            Weapon.OnWeaponReloadPressed += OnWeaponReloadPressed;
            Weapon.OnWeaponShootSucceed += OnWeaponShootSucceed;
        }

        void Update()
        {
            if (Weapon.DuringReload) return;

            desiredYaw += input.LookAxis.x * smoothAmount.x * Time.deltaTime;
            desiredYaw = Mathf.Clamp(desiredYaw, minMaxYawRotationAngle.x, minMaxYawRotationAngle.y);

            desiredPitch -= input.LookAxis.y * smoothAmount.y * Time.deltaTime;
            desiredPitch = Mathf.Clamp(desiredPitch, minMaxPitchRotationAngle.x, minMaxPitchRotationAngle.y);
        }

        void LateUpdate()
        {
            if (Weapon.DuringReload) return;

            var targetRotation = Quaternion.Euler(desiredPitch, desiredYaw, 0f);
            transform.localRotation = QuaternionExtensions.ExpDecay(
                transform.localRotation, targetRotation, smoothTime, Time.deltaTime);
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
            var punchRot = Vector3.right;
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