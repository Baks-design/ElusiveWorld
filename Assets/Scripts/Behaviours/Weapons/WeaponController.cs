using ElusiveWorld.Core.Assets.Scripts.Behaviours.Player.Movement;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Services;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Updates.Interfaces;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Updates.Types;
using ElusiveWorld.Core.Assets.Scripts.Systems.Input;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Weapons
{
    public class WeaponController : PlayerComponent, IEarlyUpdate
    {
        InputManager input;
        Weapon[] weapons;
        AimController aimController;
        bool shootHeld;

        void Start()
        {
            UpdateManager.RegisterEarlyUpdate(this);
            weapons = GetComponentsInChildren<Weapon>();
            aimController = Player.FetchComponent<AimController>();
            input = IServiceLocator.Default.GetService<InputManager>();
            input.OnShootPressed += OnShootPressed;
            input.OnShootReleased += OnShootReleased;
            input.OnReloadPressed += OnReloadPressed;
        }

        void IEarlyUpdate.EarlyUpdate()
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
            UpdateManager.UnregisterEarlyUpdate(this);
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
