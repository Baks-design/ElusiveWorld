using Assets.Scripts.Internal.Runtime.Core.Systems.Managers;
using UnityEngine;

namespace Assets.Scripts.Internal.Runtime.Core.Systems.Weapons
{
    public class WeaponGraphics : WeaponComponent<WeaponGraphics>
    {
        //Tween reloadTween;
        //Tween shootTween;
        //Vector3 initLocalPos;
        //Quaternion initLocalRot;
        LineRenderer lineRenderer;

        void Awake()
        {
            //initLocalPos = transform.localPosition;
            //initLocalRot = transform.localRotation;
            lineRenderer = GetComponentInChildren<LineRenderer>();
        }

        void OnEnable()
        {
            Weapon.OnWeaponReloadPressed += OnWeaponReloadPressed;
            Weapon.OnWeaponShootSucceed += OnWeaponShootSucceed;
        }

        void OnDisable()
        {
            Weapon.OnWeaponReloadPressed -= OnWeaponReloadPressed;
            Weapon.OnWeaponShootSucceed -= OnWeaponShootSucceed;
        }

        void OnWeaponReloadPressed()
        {
            if (Weapon.DuringReload) return;

            //var _desiredRot = transform.localEulerAngles + Vector3.forward * 360f;
            //reloadTween = transform.DOLocalRotate(_desiredRot, Weapon.Data.ReloadDuration, RotateMode.FastBeyond360)
            //     .OnStart(() => Weapon.OnWeaponReloadStarted())
            //     .OnComplete(() => Weapon.OnWeaponReloadCompleted());
        }

        void OnWeaponShootSucceed()
        {
            //shootTween = transform.DOPunchRotation(Vector3.right * 30f, 0.4f);  
            //shootTween = transform.DOPunchPosition(Vector3.back, 0.1f);
        }

        void OnLateUpdate()
        {
            if (Weapon.DuringReload) return;

            var lookVector = Weapon.CurrentAimPoint - Weapon.ProjectileAnchor.transform.position;
            var desiredLookRot = Quaternion.LookRotation(lookVector.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredLookRot, Time.deltaTime * 10f);

            var lookVectorMagnitude = lookVector.magnitude;
            lineRenderer.SetPosition(1, Vector3.forward * lookVectorMagnitude);
        }
    }
}
