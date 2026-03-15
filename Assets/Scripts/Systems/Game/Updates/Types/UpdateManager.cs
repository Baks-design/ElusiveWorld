using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Updates.Interfaces;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Updates.Callbacks;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Game.Updates.Types
{
    public static class UpdateManager
    {
        static readonly HashSet<IEarlyUpdate> earlyUpdates = new();
        static readonly HashSet<ISuperLateUpdate> superLateUpdates = new();

        public static void RegisterEarlyUpdate(IEarlyUpdate earlyUpdate) => earlyUpdates.Add(earlyUpdate);
        public static void RegisterSuperLateUpdate(ISuperLateUpdate superLateUpdate) => superLateUpdates.Add(superLateUpdate);

        public static void UnregisterEarlyUpdate(IEarlyUpdate earlyUpdate) => earlyUpdates.Remove(earlyUpdate);
        public static void UnregisterSuperLateUpdate(ISuperLateUpdate superLateUpdate) => superLateUpdates.Remove(superLateUpdate);

        [RuntimeInitializeOnLoadMethod]
        static void Init()
        {
            var defaultSystems = PlayerLoop.GetDefaultPlayerLoop();

            var mySuperLateUpdate = new PlayerLoopSystem
            {
                subSystemList = null,
                updateDelegate = OnSuperLateUpdate,
                type = typeof(AltSuperLateUpdate)
            };
            var myEarlyUpdate = new PlayerLoopSystem
            {
                subSystemList = null,
                updateDelegate = OnEarlyUpdate,
                type = typeof(AltEarlyUpdate)
            };

            var loopWithSuperLateUpdate = AddSystem<PreLateUpdate>(in defaultSystems, mySuperLateUpdate);
            var loopWithEarlyAndSuperLateUpdate = AddSystem<PreUpdate>(in loopWithSuperLateUpdate, myEarlyUpdate);

            PlayerLoop.SetPlayerLoop(loopWithEarlyAndSuperLateUpdate);
        }

        static void OnSuperLateUpdate()
        {
            // using var e = superLateUpdates.GetEnumerator();
            // while (e.MoveNext()) e.Current?.SuperLateUpdate();

            var superLateUpdatesCopy = new HashSet<ISuperLateUpdate>(superLateUpdates);
            foreach (var update in superLateUpdatesCopy)
                update?.SuperLateUpdate();
        }

        static void OnEarlyUpdate()
        {
            // using var e = earlyUpdates.GetEnumerator();
            // while (e.MoveNext()) e.Current?.EarlyUpdate();

            var earlyUpdatesCopy = new HashSet<IEarlyUpdate>(earlyUpdates);
            foreach (var update in earlyUpdatesCopy)
                update?.EarlyUpdate();
        }

        static PlayerLoopSystem AddSystem<T>(in PlayerLoopSystem loopSystem, PlayerLoopSystem systemToAdd) where T : struct
        {
            var newPlayerLoop = new PlayerLoopSystem()
            {
                loopConditionFunction = loopSystem.loopConditionFunction,
                type = loopSystem.type,
                updateDelegate = loopSystem.updateDelegate,
                updateFunction = loopSystem.updateFunction
            };

            var newSubSystemList = new List<PlayerLoopSystem>();

            foreach (var subSystem in loopSystem.subSystemList)
            {
                newSubSystemList.Add(subSystem);

                if (subSystem.type == typeof(T))
                    newSubSystemList.Add(systemToAdd);
            }

            newPlayerLoop.subSystemList = newSubSystemList.ToArray();
            return newPlayerLoop;
        }
    }
}