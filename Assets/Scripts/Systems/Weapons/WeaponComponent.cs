using UnityEngine;

namespace Assets.Scripts.Internal.Runtime.Core.Systems.Weapons
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
