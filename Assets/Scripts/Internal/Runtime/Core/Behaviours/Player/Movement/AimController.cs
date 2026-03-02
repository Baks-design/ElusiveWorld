using UnityEngine;

namespace Assets.Scripts.Internal.Runtime.Core.Behaviours.Player.Movement
{
    public class AimController : PlayerComponent
    {
        [SerializeField] float maxRayDistance = 1000f;
        [SerializeField] LayerMask collisionLayer = ~0;

        public Vector3 AimPoint { get; private set; }

        void Update()
        {
            var hitSomething = Physics.Raycast(
                transform.position, transform.forward, out var hitInfo, maxRayDistance, collisionLayer);
            AimPoint = hitSomething ? hitInfo.point : transform.forward * maxRayDistance;
        }
    }
}
