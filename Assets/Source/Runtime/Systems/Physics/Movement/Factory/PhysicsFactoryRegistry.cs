using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElusiveWorld.Internal.Runtime.Systems.Physics
{
    public class PhysicsFactoryRegistry
    {
        readonly List<IPhysicsEntityFactory> factories = new();
        static PhysicsFactoryRegistry instance;

        public static PhysicsFactoryRegistry Instance => instance ??= new();

        PhysicsFactoryRegistry() => RegisterDefaultFactories();

        void RegisterDefaultFactories()
        {
            RegisterFactory(new RigidbodyEntityFactory());
            RegisterFactory(new CharacterControllerEntityFactory());
        }

        public void RegisterFactory(IPhysicsEntityFactory factory) => factories.Add(factory);

        public PhysicsService CreateService(GameObject target, RaycastConfig config = null)
        {
            foreach (var factory in factories)
                if (factory.CanCreateEntity(target))
                    return CreateServiceFromFactory(factory, target, config);

            Debug.LogError($"No suitable physics factory found for {target.name}");
            return null;
        }

        PhysicsService CreateServiceFromFactory(IPhysicsEntityFactory factory, GameObject target, RaycastConfig config)
        {
            var entity = factory.CreateEntity(target);
            if (entity == null) return null;

            IMovementStrategy movementStrategy = null;
            IJumpHandler jumpHandler = null;
            IRaycastService raycastService = null;

            // Get type-specific strategies
            switch (factory)
            {
                case RigidbodyEntityFactory rbFactory:
                    movementStrategy = rbFactory.CreateMovementStrategy();
                    jumpHandler = rbFactory.CreateJumpHandler();
                    raycastService = rbFactory.CreateRaycastService(entity);
                    break;
                case CharacterControllerEntityFactory ccFactory:
                    movementStrategy = ccFactory.CreateMovementStrategy();
                    jumpHandler = ccFactory.CreateJumpHandler();
                    raycastService = ccFactory.CreateRaycastService(entity);
                    break;
            }

            return new PhysicsService(entity, movementStrategy, jumpHandler, raycastService);
        }
    }
}