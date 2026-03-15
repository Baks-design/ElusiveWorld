using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Updates.Interfaces;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Updates.Types;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Player.Movement
{
    public class AimController : PlayerComponent, IEarlyUpdate
    {
        [SerializeField] float maxRayDistance = 1000f;
        [SerializeField] LayerMask collisionLayer = ~0;

        public Vector3 AimPoint { get; private set; }

        void OnEnable() => UpdateManager.RegisterEarlyUpdate(this);

        void OnDisable() => UpdateManager.UnregisterEarlyUpdate(this);

        void IEarlyUpdate.EarlyUpdate()
        {
            var hitSomething = Physics.Raycast(
                transform.position, transform.forward, out var hitInfo, maxRayDistance, collisionLayer);
            AimPoint = hitSomething ? hitInfo.point : transform.forward * maxRayDistance;
        }
    }
}
