using System.Collections.Generic;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Updates.Variable.Interfaces;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Updates.Variable.Types;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Game.Updates.Variable
{
    public static class UpdateManager
    {
        static readonly HashSet<IEarlyUpdate> earlyUpdates = new();
        static readonly HashSet<IUpdate> updates = new();
        static readonly HashSet<ILateUpdate> lateUpdates = new();
        static readonly List<IEarlyUpdate> earlyUpdatesTemp = new();
        static readonly List<IUpdate> updatesTemp = new();
        static readonly List<ILateUpdate> lateUpdatesTemp = new();
        static PlayerLoopSystem myEarlyUpdate;
        static PlayerLoopSystem myUpdate;
        static PlayerLoopSystem myLateUpdate;

        public static void RegisterEarlyUpdate(IEarlyUpdate earlyUpdate) => earlyUpdates.Add(earlyUpdate);
        public static void RegisterUpdate(IUpdate update) => updates.Add(update);
        public static void RegisterLateUpdate(ILateUpdate lateUpdate) => lateUpdates.Add(lateUpdate);

        public static void UnregisterEarlyUpdate(IEarlyUpdate earlyUpdate) => earlyUpdates.Remove(earlyUpdate);
        public static void UnregisterUpdate(IUpdate update) => updates.Remove(update);
        public static void UnregisterLateUpdate(ILateUpdate lateUpdate) => lateUpdates.Remove(lateUpdate);

        [RuntimeInitializeOnLoadMethod]
        static void Init()
        {
            var defaultSystems = PlayerLoop.GetDefaultPlayerLoop();

            myEarlyUpdate = new PlayerLoopSystem
            {
                subSystemList = null,
                updateDelegate = OnEarlyUpdate,
                type = typeof(EarlyUpdate)
            };
            myUpdate = new PlayerLoopSystem
            {
                subSystemList = null,
                updateDelegate = OnUpdate,
                type = typeof(CustomUpdate)
            };
            myLateUpdate = new PlayerLoopSystem
            {
                subSystemList = null,
                updateDelegate = OnLateUpdate,
                type = typeof(CustomLateUpdate)
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
                if (subSystem.type != typeof(Update))
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
                if (newSystemSubsystem.type != typeof(Update.ScriptRunBehaviourUpdate))
                    newSystemSubSystemList.Add(newSystemSubsystem);
                else
                {
                    newSystemSubSystemList.Add(myEarlyUpdate);
                    newSystemSubSystemList.Add(myUpdate);
                    newSystemSubSystemList.Add(myLateUpdate);
                }
            }

            newSubSystem.subSystemList = newSystemSubSystemList.ToArray();
            return newSubSystem;
        }

        static void OnEarlyUpdate()
        {
            earlyUpdatesTemp.Clear();
            earlyUpdatesTemp.AddRange(earlyUpdates);

            foreach (var update in earlyUpdatesTemp)
            {
                try { update?.EarlyUpdate(); }
                catch (System.Exception e) { Debug.LogError($"Error in EarlyUpdate: {e}"); }
            }
        }

        static void OnUpdate()
        {
            updatesTemp.Clear();
            updatesTemp.AddRange(updates);

            foreach (var update in updatesTemp)
            {
                try { update?.Update(); }
                catch (System.Exception e) { Debug.LogError($"Error in Update: {e}"); }
            }
        }

        static void OnLateUpdate()
        {
            lateUpdatesTemp.Clear();
            lateUpdatesTemp.AddRange(lateUpdates);

            foreach (var update in lateUpdatesTemp)
            {
                try { update?.LateUpdate(); }
                catch (System.Exception e) { Debug.LogError($"Error in LateUpdate: {e}"); }
            }
        }
    }
}