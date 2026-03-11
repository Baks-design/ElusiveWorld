using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Weapons
{
    public class WeaponComponent<T> : MonoBehaviour where T : WeaponComponent<T>
    {
        bool cached;
        Weapon weapon;

        public Weapon Weapon
        {
            get
            {
                if (!cached)
                {
                    cached = true;
                    weapon = GetComponentInParent<Weapon>();
                }
                return weapon;
            }
        }
    }
}
