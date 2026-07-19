using UnityEngine;

namespace ElusiveWorld.Internal.Runtime.Systems.Physics
{
    public class RigidbodyRaycastService : BaseRaycastService
    {
        readonly Rigidbody rigidbody;
        readonly Collider collider;
        float colliderHeight;
        float colliderRadius;

        public RigidbodyRaycastService(IPhysicsEntity entity, RaycastConfig config) : base(entity, config)
        {
            // Get the Rigidbody from the entity's transform
            var monoBehaviour = entity as MonoBehaviour;
            if (monoBehaviour != null)
            {
                rigidbody = monoBehaviour.GetComponent<Rigidbody>();
                collider = monoBehaviour.GetComponent<Collider>();
            }
            else
            {
                // If entity is not a MonoBehaviour, try to get from transform
                rigidbody = entity.Transform.GetComponent<Rigidbody>();
                collider = entity.Transform.GetComponent<Collider>();
            }

            if (rigidbody == null)
            {
                Debug.LogError("RigidbodyRaycastService requires a Rigidbody component");
                return;
            }

            InitializeColliderDimensions();
        }

        void InitializeColliderDimensions()
        {
            if (collider == null) return;

            switch (collider)
            {
                case CapsuleCollider capsule:
                    colliderHeight = capsule.height;
                    colliderRadius = capsule.radius;
                    break;
                case SphereCollider sphere:
                    colliderHeight = sphere.radius * 2f;
                    colliderRadius = sphere.radius;
                    break;
                case BoxCollider box:
                    colliderHeight = box.size.y;
                    colliderRadius = Mathf.Min(box.size.x, box.size.z) * 0.5f;
                    break;
                default:
                    // Default dimensions if no recognized collider
                    colliderHeight = 2f;
                    colliderRadius = 0.5f;
                    break;
            }
        }

        public override RaycastResult CheckGround(float maxDistance = 0.1f)
        {
            var checkDistance = maxDistance > 0f ? maxDistance : Config.groundCheckDistance;
            var origin = Transform.position + Config.groundCheckOffset;
            var didHit = UnityEngine.Physics.SphereCast(
                origin, colliderRadius * 0.9f, Vector3.down, out var hit,
                checkDistance + colliderHeight * 0.5f - colliderRadius,
                Config.groundLayers, QueryTriggerInteraction.Ignore
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
            var origin = Transform.position + Vector3.up * (colliderHeight * 0.5f);

            var didHit = UnityEngine.Physics.SphereCast(
                origin, Config.ceilingRadius, Vector3.up, out var hit, checkDistance,
                Config.groundLayers, QueryTriggerInteraction.Ignore
            );

            return didHit ? CreateRaycastResult(hit) : RaycastResult.Empty;
        }

        public override RaycastResult CheckWall(Vector3 direction, float maxDistance = 0.1f)
        {
            var checkDistance = maxDistance > 0f ? maxDistance : Config.wallCheckDistance;
            direction = direction.normalized;

            // Multiple raycasts around the character for better wall detection
            var angleStep = Config.wallCheckAngle / Config.wallCheckRays;
            var closestHit = RaycastResult.Empty;
            var closestDistance = float.MaxValue;

            for (var i = 0; i < Config.wallCheckRays; i++)
            {
                var angle = i * angleStep * Mathf.Deg2Rad;
                var rayDirection = Quaternion.Euler(0f, angle * Mathf.Rad2Deg, 0f) * direction;

                if (UnityEngine.Physics.Raycast(
                    Transform.position, rayDirection, out var hit, checkDistance,
                    Config.groundLayers, QueryTriggerInteraction.Ignore))
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
            var groundOrigin = Transform.position + Config.groundCheckOffset;
            Gizmos.DrawWireSphere(groundOrigin + Vector3.down * Config.groundCheckDistance, colliderRadius * 0.9f);

            // Ceiling check gizmo
            Gizmos.color = Color.cyan;
            var ceilingOrigin = Transform.position + Vector3.up * (colliderHeight * 0.5f);
            Gizmos.DrawWireSphere(ceilingOrigin, Config.ceilingRadius);

            // Wall check gizmos
            Gizmos.color = Config.wallGizmoColor;
            for (var i = 0; i < Config.wallCheckRays; i++)
            {
                var angle = i * (360f / Config.wallCheckRays) * Mathf.Deg2Rad;
                var direction = new Vector3(Mathf.Sin(angle), 0f, Mathf.Cos(angle));
                Gizmos.DrawRay(Transform.position, direction * Config.wallCheckDistance);
            }
        }
    }
}