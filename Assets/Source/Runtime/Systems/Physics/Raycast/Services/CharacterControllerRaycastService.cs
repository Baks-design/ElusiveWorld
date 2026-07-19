using UnityEngine;

namespace ElusiveWorld.Internal.Runtime.Systems.Physics
{
    public class CharacterControllerRaycastService : BaseRaycastService
    {
        readonly CharacterController controller;
        readonly float skinWidth = 0.08f;

        public CharacterControllerRaycastService(IPhysicsEntity entity, RaycastConfig config) : base(entity, config)
        {
            // Get the CharacterController from the entity's transform
            var monoBehaviour = entity as MonoBehaviour;
            if (monoBehaviour != null)
                controller = monoBehaviour.GetComponent<CharacterController>();
            else
                // If entity is not a MonoBehaviour, try to get from transform
                controller = entity.Transform.GetComponent<CharacterController>();

            if (controller == null)
            {
                Debug.LogError("CharacterControllerRaycastService requires a CharacterController component");
                return;
            }

            skinWidth = controller.skinWidth;
        }

        public override RaycastResult CheckGround(float maxDistance = 0.1f)
        {
            var checkDistance = maxDistance > 0 ? maxDistance : Config.groundCheckDistance;
            var origin = Transform.position + Config.groundCheckOffset;

            // Use capsule cast matching CharacterController bounds
            var point1 = origin + Vector3.up * controller.radius;
            var point2 = origin + Vector3.up * (controller.height - controller.radius);

            var didHit = UnityEngine.Physics.CapsuleCast(
                point1, point2, controller.radius - skinWidth, Vector3.down, out var hit,
                checkDistance, Config.groundLayers, QueryTriggerInteraction.Ignore
            );

            if (didHit)
            {
                GroundNormal = hit.normal;
                GroundCollider = hit.collider;
                CheckSlopeAngle();

                return new RaycastResult
                {
                    Hit = true,
                    Point = hit.point,
                    Normal = hit.normal,
                    Distance = hit.distance,
                    Collider = hit.collider,
                    HitTransform = hit.transform
                };
            }

            GroundNormal = Vector3.up;
            GroundCollider = null;
            IsOnSlope = false;

            return RaycastResult.Empty;
        }

        public override RaycastResult CheckCeiling(float maxDistance = 0.1f)
        {
            var checkDistance = maxDistance > 0f ? maxDistance : Config.ceilingCheckDistance;
            var origin = Transform.position + Vector3.up * (controller.height * 0.5f);

            var didHit = UnityEngine.Physics.SphereCast(
                origin, controller.radius - skinWidth, Vector3.up, out var hit,
                checkDistance, Config.groundLayers, QueryTriggerInteraction.Ignore
            );

            return didHit ? CreateRaycastResult(hit) : RaycastResult.Empty;
        }

        public override RaycastResult CheckWall(Vector3 direction, float maxDistance = 0.1f)
        {
            var checkDistance = maxDistance > 0f ? maxDistance : Config.wallCheckDistance;
            direction = direction.normalized;

            // Multiple capsule casts around the character
            var angleStep = Config.wallCheckAngle / Config.wallCheckRays;
            var closestHit = RaycastResult.Empty;
            var closestDistance = float.MaxValue;

            var bottomPoint = Transform.position + Vector3.up * controller.radius;
            var topPoint = Transform.position + Vector3.up * (controller.height - controller.radius);

            for (var i = 0; i < Config.wallCheckRays; i++)
            {
                var angle = i * angleStep * Mathf.Deg2Rad;
                var rayDirection = Quaternion.Euler(0f, angle * Mathf.Rad2Deg, 0f) * direction;
                if (UnityEngine.Physics.CapsuleCast(
                    bottomPoint, topPoint, controller.radius - skinWidth,
                    rayDirection, out var hit,
                    checkDistance, Config.groundLayers, QueryTriggerInteraction.Ignore))
                {
                    if (hit.distance < closestDistance)
                    {
                        closestDistance = hit.distance;
                        closestHit = CreateRaycastResult(hit);
                    }
                }
            }

            return closestHit;
        }

        public override RaycastResult CheckSlope(out float slopeAngle)
        {
            if (!Entity.IsGrounded)
            {
                slopeAngle = 0f;
                return RaycastResult.Empty;
            }

            var origin = Transform.position + Config.groundCheckOffset;
            if (UnityEngine.Physics.Raycast(
                origin, Vector3.down, out var hit, Config.slopeCheckDistance,
                Config.groundLayers, QueryTriggerInteraction.Ignore))
            {
                slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
                if (slopeAngle > 0f && slopeAngle <= Config.maxSlopeAngle)
                {
                    IsOnSlope = true;
                    SlopeAngle = slopeAngle;
                    return CreateRaycastResult(hit);
                }
            }

            slopeAngle = 0f;
            IsOnSlope = false;
            return RaycastResult.Empty;
        }

        void CheckSlopeAngle()
        {
            SlopeAngle = Vector3.Angle(GroundNormal, Vector3.up);
            IsOnSlope = SlopeAngle > 0.1f && SlopeAngle <= Config.maxSlopeAngle;
        }

        RaycastResult CreateRaycastResult(RaycastHit hit) => new()
        {
            Hit = true,
            Point = hit.point,
            Normal = hit.normal,
            Distance = hit.distance,
            Collider = hit.collider,
            HitTransform = hit.transform
        };

        public void DrawGizmos()
        {
            if (!Config.drawGizmos) return;

            // Ground check gizmo
            Gizmos.color = Config.groundGizmoColor;
            Vector3 groundOrigin = Transform.position + Config.groundCheckOffset;
            Gizmos.DrawWireSphere(groundOrigin + Vector3.down * Config.groundCheckDistance, controller.radius - skinWidth);

            // Ceiling check gizmo
            Gizmos.color = Color.cyan;
            Vector3 ceilingOrigin = Transform.position + Vector3.up * (controller.height * 0.5f);
            Gizmos.DrawWireSphere(ceilingOrigin, controller.radius - skinWidth);

            // Character bounds gizmo
            Gizmos.color = Color.grey;
            Vector3 boundsCenter = Transform.position + controller.center;
            Gizmos.DrawWireCube(boundsCenter, new Vector3(controller.radius * 2f, controller.height, controller.radius * 2f));
        }
    }
}