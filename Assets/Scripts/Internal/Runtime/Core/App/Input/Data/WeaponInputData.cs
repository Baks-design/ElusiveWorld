using UnityEngine;

namespace VHS
{
    [CreateAssetMenu(menuName = "Data/WeaponInputData")]
    public class WeaponInputData : ScriptableObject
    {
        bool isShoot;
        bool isReload;

        public bool IsShoot
        {
            get => isShoot;
            set => isShoot = value;
        }
        public bool IsReload
        {
            get => isReload;
            set => isReload = value;
        }

        public void ResetInput()
        {
            isShoot = false;
            isReload = false;
        }
    }
}
