using UnityEngine;

namespace VHS
{
    public class WeaponController : MonoBehaviour
    {
        [SerializeField] WeaponInputData weaponInputData;
        Weapon[] weapons;

        void Start() => weapons = GetComponentsInChildren<Weapon>();

        void Update()
        {
            if (weaponInputData.IsShoot)
                foreach (Weapon weapon in weapons)
                    weapon.OnShootButtonPressed();
                    
            if (weaponInputData.IsShoot)
                foreach (Weapon weapon in weapons)
                    weapon.OnShootButtonHeld();

            if (weaponInputData.IsShoot)
                foreach (Weapon weapon in weapons)
                    weapon.OnShootButtonReleased();

            if (weaponInputData.IsReload)
                foreach (Weapon weapon in weapons)
                    weapon.OnReloadButtonPressed();
        }
    }
}
