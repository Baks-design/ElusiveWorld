using System;
using System.Collections.Generic;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Updates.Fixed.Interfaces;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Updates.Fixed.Types;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Game.Updates.Fixed
{
    public static class FixedUpdateManager
    {
        static readonly HashSet<IEarlyFixedUpdate> earlyFixedUpdates = new();
        static readonly HashSet<IFixedUpdate> fixedUpdates = new();
        static readonly HashSet<ILateFixedUpdate> lateFixedUpdates = new();
        static readonly List<IEarlyFixedUpdate> earlyFixedUpdatesTemp = new();
        static readonly List<IFixedUpdate> fixedUpdatesTemp = new();
        static readonly List<ILateFixedUpdate> lateFixedUpdatesTemp = new();
        static PlayerLoopSystem myEarlyFixedUpdate;
        static PlayerLoopSystem myFixedUpdate;
        static PlayerLoopSystem myLateFixedUpdate;

        public static void RegisterEarlyFixedUpdate(IEarlyFixedUpdate earlyUpdate) => earlyFixedUpdates.Add(earlyUpdate);
        public static void RegisterFixedUpdate(IFixedUpdate fixedUpdate) => fixedUpdates.Add(fixedUpdate);
        public static void RegisterLateFixedUpdate(ILateFixedUpdate lateUpdate) => lateFixedUpdates.Add(lateUpdate);

        public static void UnregisterEarlyFixedUpdate(IEarlyFixedUpdate earlyUpdate) => earlyFixedUpdates.Remove(earlyUpdate);
        public static void UnregisterFixedUpdate(IFixedUpdate fixedUpdate) => fixedUpdates.Remove(fixedUpdate);
        public static void UnregisterLateFixedUpdate(ILateFixedUpdate lateUpdate) => lateFixedUpdates.Remove(lateUpdate);

        [RuntimeInitializeOnLoadMethod]
        static void Init()
        {
            var defaultSystems = PlayerLoop.GetDefaultPlayerLoop();

            myEarlyFixedUpdate = new PlayerLoopSystem
            {
                subSystemList = null,
                updateDelegate = OnEarlyFixedUpdate,
                type = typeof(CustomEarlyFixedUpdate)
            };
            myFixedUpdate = new PlayerLoopSystem
            {
                subSystemList = null,
                updateDelegate = OnFixedUpdate,
                type = typeof(CustomFixedUpdate)
            };
            myLateFixedUpdate = new PlayerLoopSystem
            {
                subSystemList = null,
                updateDelegate = OnLateFixedUpdate,
                type = typeof(CustomLateFixedUpdate)
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
                    newSystemSubSystemList.Add(myEarlyFixedUpdate);
                    newSystemSubSystemList.Add(myFixedUpdate);
                    newSystemSubSystemList.Add(myLateFixedUpdate);
                }
            }

            newSubSystem.subSystemList = newSystemSubSystemList.ToArray();
            return newSubSystem;
        }

        static void OnEarlyFixedUpdate()
        {
            earlyFixedUpdatesTemp.Clear();
            earlyFixedUpdatesTemp.AddRange(earlyFixedUpdates);

            foreach (var update in earlyFixedUpdatesTemp)
            {
                try { update?.EarlyFixedUpdate(Time.fixedDeltaTime); }
                catch (Exception e) { Debug.LogError($"Error in EarlyFixedUpdate: {e}"); }
            }
        }

        static void OnFixedUpdate()
        {
            fixedUpdatesTemp.Clear();
            fixedUpdatesTemp.AddRange(fixedUpdates);

            foreach (var update in fixedUpdatesTemp)
            {
                try { update?.FixedUpdate(Time.fixedDeltaTime); }
                catch (Exception e) { Debug.LogError($"Error in FixedUpdate: {e}"); }
            }
        }

        static void OnLateFixedUpdate()
        {
            lateFixedUpdatesTemp.Clear();
            lateFixedUpdatesTemp.AddRange(lateFixedUpdates);

            foreach (var update in lateFixedUpdatesTemp)
            {
                try { update?.LateFixedUpdate(Time.fixedDeltaTime); }
                catch (Exception e) { Debug.LogError($"Error in LateFixedUpdate: {e}"); }
            }
        }
    }
}