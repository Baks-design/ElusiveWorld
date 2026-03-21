using UnityEngine;
using Unity.Cinemachine;
using ElusiveWorld.Core.Assets.Scripts.Systems.Input;
using ElusiveWorld.Core.Assets.Scripts.Utils.Extensions;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Services;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Updates.Variable.Interfaces;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Updates.Variable;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Weapons
{
    public class WeaponGraphics : WeaponComponent<WeaponGraphics>, ILateUpdate
    {
        [SerializeField] Vector2 smoothAmount = new(30f, 30f);
        [SerializeField, MinMaxRangeSlider(-90f, 90f)] Vector2 minMaxYawRotationAngle = new(-30f, 30f);
        [SerializeField, MinMaxRangeSlider(-90f, 90f)] Vector2 minMaxPitchRotationAngle = new(-30f, 30f);
        [SerializeField] float smoothTime = 10f;
        InputManager input;
        float desiredYaw;
        float desiredPitch;

        void OnEnable() => UpdateManager.RegisterLateUpdate(this);

        void Start() => input = IServiceLocator.Default.GetService<InputManager>();

        void ILateUpdate.LateUpdate()
        {
            if (Weapon.DuringReload) return;

            desiredYaw += input.LookAxis.x * smoothAmount.x * Time.deltaTime;
            desiredYaw = Mathf.Clamp(desiredYaw, minMaxYawRotationAngle.x, minMaxYawRotationAngle.y);

            desiredPitch -= input.LookAxis.y * smoothAmount.y * Time.deltaTime;
            desiredPitch = Mathf.Clamp(desiredPitch, minMaxPitchRotationAngle.x, minMaxPitchRotationAngle.y);

            var targetRotation = Quaternion.Euler(desiredPitch, desiredYaw, 0f);
            transform.localRotation = transform.localRotation.ExpDecay(
                targetRotation, smoothTime, Time.deltaTime);
        }

        void OnDisable() => UpdateManager.UnregisterLateUpdate(this);
    }
}