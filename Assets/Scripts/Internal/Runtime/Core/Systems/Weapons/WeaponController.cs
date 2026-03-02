using Assets.Scripts.Internal.Runtime.Core.App.Input;
using Assets.Scripts.Internal.Runtime.Core.Behaviours.Player.Movement;

namespace Assets.Scripts.Internal.Runtime.Core.Systems.Weapons
{
    public class WeaponController : PlayerComponent
    {
        Weapon[] weapons;
        AimController aimController;
        bool shootHeld;

        void Awake() => weapons = GetComponentsInChildren<Weapon>();

        void OnEnable()
        {
            InputManager.OnShootPressed += OnShootPressed;
            InputManager.OnShootReleased += OnShootReleased;
            InputManager.OnReloadPressed += OnReloadPressed;
        }

        void Start() => aimController = Player.FetchComponent<AimController>();

        void Update() => OnWeaponTick();

        void OnDisable()
        {
            InputManager.OnShootPressed -= OnShootPressed;
            InputManager.OnShootReleased -= OnShootReleased;
            InputManager.OnReloadPressed -= OnReloadPressed;
        }

        void OnShootPressed()
        {
            shootHeld = true;
            foreach (var weapon in weapons)
                weapon.OnShootButtonPressed();
        }

        void OnShootReleased()
        {
            shootHeld = false;
            foreach (var weapon in weapons)
                weapon.OnShootButtonReleased();
        }

        void OnReloadPressed()
        {
            foreach (var weapon in weapons)
                weapon.OnReloadButtonPressed();
        }

        void OnWeaponTick()
        {
            if (shootHeld)
                foreach (var weapon in weapons)
                    weapon.OnShootButtonHeld();
            foreach (var weapon in weapons)
                weapon.CurrentAimPoint = aimController.AimPoint;
        }
    }
}
