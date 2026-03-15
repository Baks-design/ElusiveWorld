using System.Collections.Generic;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Updates.Callbacks;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Updates.Interfaces;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Game.Updates.Types
{
    public static class FixedUpdateManager
    {
        static readonly HashSet<IEarlyFixedUpdate> earlyFixedUpdates = new();
        static readonly HashSet<ILateFixedUpdate> lateFixedUpdates = new();
        static PlayerLoopSystem altEarlyFixedUpdate;
        static PlayerLoopSystem altLateFixedUpdate;

        public static void RegisterEarlyFixedUpdate(IEarlyFixedUpdate earlyUpdate) => earlyFixedUpdates.Add(earlyUpdate);
        public static void RegisterLateFixedUpdate(ILateFixedUpdate superLateUpdate) => lateFixedUpdates.Add(superLateUpdate);

        public static void UnregisterEarlyFixedUpdate(IEarlyFixedUpdate earlyUpdate) => earlyFixedUpdates.Remove(earlyUpdate);
        public static void UnregisterLateFixedUpdate(ILateFixedUpdate superLateUpdate) => lateFixedUpdates.Remove(superLateUpdate);

        [RuntimeInitializeOnLoadMethod]
        static void Init()
        {
            var defaultSystems = PlayerLoop.GetDefaultPlayerLoop();

            altEarlyFixedUpdate = new PlayerLoopSystem
            {
                subSystemList = null,
                updateDelegate = OnEarlyFixedUpdate,
                type = typeof(AltEarlyFixedUpdate)
            };
            altLateFixedUpdate = new PlayerLoopSystem
            {
                subSystemList = null,
                updateDelegate = OnLateFixedUpdate,
                type = typeof(AltLateFixedUpdate)
            };

            var newPlayerLoop = new PlayerLoopSystem()
            {
                loopConditionFunction = defaultSystems.loopConditionFunction,
                type = defaultSystems.type,
                updateDelegate = defaultSystems.updateDelegate,
                updateFunction = defaultSystems.updateFunction
            };

            var newSubSystemList = new List<PlayerLoopSystem>();
            foreach (var subSystem in defaultSystems.subSystemList)
            {
                if (subSystem.type != typeof(FixedUpdate))
                    newSubSystemList.Add(subSystem);
                else
                {
                    var newSubSystem = CreateNewSubsystem(subSystem);
                    newSubSystemList.Add(newSubSystem);
                }
            }

            newPlayerLoop.subSystemList = newSubSystemList.ToArray();

            PlayerLoop.SetPlayerLoop(newPlayerLoop);

            return;
        }

        static void OnEarlyFixedUpdate()
        {
            using var e = earlyFixedUpdates.GetEnumerator();
            while (e.MoveNext()) e.Current?.EarlyFixedUpdate();
        }

        static void OnLateFixedUpdate()
        {
            using var e = lateFixedUpdates.GetEnumerator();
            while (e.MoveNext()) e.Current?.LateFixedUpdate();
        }

        static PlayerLoopSystem CreateNewSubsystem(PlayerLoopSystem subSystem)
        {
            var newSubSystem = new PlayerLoopSystem()
            {
                loopConditionFunction = subSystem.loopConditionFunction,
                type = subSystem.type,
                updateDelegate = subSystem.updateDelegate,
                updateFunction = subSystem.updateFunction
            };

            var newSystemSubSystemList = new List<PlayerLoopSystem>();

            foreach (var newSystemSubsystem in subSystem.subSystemList)
            {
                if (newSystemSubsystem.type != typeof(FixedUpdate.ScriptRunBehaviourFixedUpdate))
                    newSystemSubSystemList.Add(newSystemSubsystem);
                else
                {
                    newSystemSubSystemList.Add(altEarlyFixedUpdate);
                    newSystemSubSystemList.Add(newSystemSubsystem);
                    newSystemSubSystemList.Add(altLateFixedUpdate);
                }
            }

            newSubSystem.subSystemList = newSystemSubSystemList.ToArray();
            return newSubSystem;
        }
    }
}