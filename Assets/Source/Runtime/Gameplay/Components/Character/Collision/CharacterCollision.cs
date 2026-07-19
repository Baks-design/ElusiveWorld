using ElusiveWorld.Internal.Runtime.Systems.Physics;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Audio.Data
{
    public class CharacterCollision : MonoBehaviour
    {
        [SerializeField] CharacterController controller;
        [SerializeField] RaycastConfig raycastConfig;
        PhysicsService physicsService;

        void Awake()
        {
            raycastConfig ??= new RaycastConfig();
            physicsService = controller.gameObject.CreatePhysicsService(raycastConfig);
            if (physicsService == null)
            {
                Debug.LogError("Failed to create physics service. " +
                    "Ensure GameObject has Rigidbody or CharacterController component.");
                enabled = false;
            }
        }
    }
}