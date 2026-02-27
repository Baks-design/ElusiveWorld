using UnityEngine;

namespace VHS
{
    public class WeaponGraphics : WeaponComponent<WeaponGraphics> //TODO:Change Dotween
    {
        //Tween reloadTween;
        //Tween shootTween;

        // void OnEnable()
        // {
        //     Weapon.OnWeaponReloadPressed += OnWeaponReloadPressed;
        //     Weapon.OnWeaponShootSucceed += OnWeaponShootSucceed;
        // }

        // void OnDisable()
        // {
        //     Weapon.OnWeaponReloadPressed -= OnWeaponReloadPressed;
        //     Weapon.OnWeaponShootSucceed -= OnWeaponShootSucceed;
        // }

        // void OnWeaponReloadPressed() =>
        //     reloadTween = transform.DOLocalRotate(
        //         Vector3.right * 360f,
        //         Weapon.Data.ReloadDuration,
        //         RotateMode.FastBeyond360).OnComplete(() => Weapon.OnWeaponReloadCompleted());

        // void OnWeaponShootSucceed() => shootTween = transform.DOPunchRotation(Vector3.right * 30f, 1f);
    }
}
