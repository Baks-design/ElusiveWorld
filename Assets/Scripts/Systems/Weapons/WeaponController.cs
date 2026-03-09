using Assets.Scripts.Internal.Runtime.Core.App.Input;
using Assets.Scripts.Internal.Runtime.Core.Behaviours.Player.Movement;
using UnityEngine;

namespace Assets.Scripts.Internal.Runtime.Core.Systems.Weapons
{
    public class WeaponController : PlayerComponent
    {
        [SerializeField] InputReader input;
        Weapon[] weapons;
        AimController aimController;
        bool shootHeld;

        void Awake() => weapons = GetComponentsInChildren<Weapon>();

        void OnEnable()
        {
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
