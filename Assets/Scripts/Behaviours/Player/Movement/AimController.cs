using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Updates.Variable;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Updates.Variable.Interfaces;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Player.Movement
{
    public class AimController : PlayerComponent, IUpdate
    {
        [SerializeField] float maxRayDistance = 1000f;
        [SerializeField] LayerMask collisionLayer = ~0;

        public Vector3 AimPoint { get; private set; }

        void Start() => UpdateManager.RegisterUpdate(this);

        void IUpdate.Update()
        {
            var hitSomething = Physics.Raycast(
                transform.position, transform.forward, out var hitInfo, maxRayDistance, collisionLayer);
            AimPoint = hitSomething ? hitInfo.point : transform.forward * maxRayDistance;
        }

        void OnDisable() => UpdateManager.UnregisterUpdate(this);
    }
}
