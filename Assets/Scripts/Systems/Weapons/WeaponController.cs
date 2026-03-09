using ElusiveWorld.Core.Assets.Scripts.Behaviours.Player.Movement;
using ElusiveWorld.Core.Assets.Scripts.Systems.Input;
using ElusiveWorld.Core.Assets.Scripts.Utils.Services;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Weapons
{
    public class WeaponController : PlayerComponent 
    {
        InputManager input;
        Weapon[] weapons;
        AimController aimController;
        bool shootHeld;

        void Awake() => weapons = GetComponentsInChildren<Weapon>();

        void OnEnable()
        {
            input = IServiceLocator.Default.GetService<InputManager>();
            input.OnShootPressed += OnShootPressed;
            input.OnShootReleased += OnShootReleased;
            input.OnReloadPressed += OnReloadPressed;
        }

        void Start() => aimController = Player.FetchComponent<AimController>();

        void Update()
        {
            if (shootHeld)
                foreach (var weapon in weapons)
                    weapon.OnShootButtonHeld();

            foreach (var weapon in weapons)
                weapon.CurrentAimPoint = aimController.AimPoint;
        }

        void OnDisable()
        {
            input.OnShootPressed -= OnShootPressed;
            input.OnShootReleased -= OnShootReleased;
            input.OnReloadPressed -= OnReloadPressed;
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
    }
}
