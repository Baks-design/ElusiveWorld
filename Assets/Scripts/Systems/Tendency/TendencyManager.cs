using System;
using System.Collections.Generic;
using ElusiveWorld.Core.Assets.Scripts.Systems.Tendency.Interfaces;
using ElusiveWorld.Core.Assets.Scripts.Utils.Services;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Tendency
{
    public class TendencyManager : MonoBehaviour, IService, ITendencySystem
    {
        [Header("Settings")]
        [SerializeField] float globalTendency = 0f;
        [SerializeField] float minTendency = -3f;
        [SerializeField] float maxTendency = 3f;
        [Header("Configuration")]
        [SerializeField] List<TendencyThreshold> thresholds;
        [SerializeField] List<ActionImpact> actionImpacts;
        readonly Dictionary<string, float> regionalTendencies = new();
        readonly List<ITendencyEffect> activeEffects = new();

        public float GlobalTendency => globalTendency;

        public event Action<TendencyState> OnStateChanged = delegate { };
        public event Action<string, float> OnActionRegistered = delegate { };
        public event Action<TendencyState> OnTendencyChanged = delegate { };

        public void Initialize() { if (thresholds.Count == 0) InitializeDefaultThresholds(); }

        public void Dispose() { }

        void InitializeDefaultThresholds() => thresholds = new List<TendencyThreshold>
            {
                new() { state = TendencyState.PureBlack, minValue = -3f, maxValue = -2.1f },
                new() { state = TendencyState.Black, minValue = -2f, maxValue = -1.1f },
                new() { state = TendencyState.Dark, minValue = -1f, maxValue = -0.1f },
                new() { state = TendencyState.Neutral, minValue = 0f, maxValue = 0f },
                new() { state = TendencyState.Light, minValue = 0.1f, maxValue = 1f },
                new() { state = TendencyState.White, minValue = 1.1f, maxValue = 2f },
                new() { state = TendencyState.PureWhite, minValue = 2.1f, maxValue = 3f }
            };

        public void AddTendency(float amount)
        {
            globalTendency = Mathf.Clamp(GlobalTendency + amount, minTendency, maxTendency);

            var oldState = GetCurrentState();
            var newState = GetCurrentState();
            if (oldState != newState)
            {
                OnTendencyChanged.Invoke(newState);
                OnStateChanged.Invoke(newState);
                UpdateEffects(newState);
            }
        }

        public void RegisterAction(string actionId, float customImpact = 0f)
        {
            var impact = actionImpacts.Find(x => x.actionId == actionId);
            if (impact != null)
            {
                var finalImpact = customImpact != 0f ? customImpact : impact.impact;

                if (impact.isRegional)
                {
                    var region = GetCurrentRegion();
                    AddRegionalTendency(region, finalImpact);
                }
                else
                    AddTendency(finalImpact);

                OnActionRegistered.Invoke(actionId, finalImpact);
            }
        }

        public void RegisterEffect(ITendencyEffect effect)
        {
            activeEffects.Add(effect);
            if (effect.RequiredState == GetCurrentState())
                effect.Apply();
        }

        public void UnregisterEffect(ITendencyEffect effect)
        {
            if (activeEffects.Remove(effect))
                effect.Remove();
        }

        void UpdateEffects(TendencyState newState)
        {
            foreach (var effect in activeEffects)
            {
                if (effect.RequiredState == newState)
                    effect.Apply();
                else
                    effect.Remove();
            }
        }

        public TendencyState GetCurrentState()
        {
            foreach (var threshold in thresholds)
                if (GlobalTendency >= threshold.minValue && GlobalTendency <= threshold.maxValue)
                    return threshold.state;

            return TendencyState.Neutral;
        }

        void AddRegionalTendency(string region, float amount)
        {
            if (!regionalTendencies.ContainsKey(region))
                regionalTendencies[region] = 0f;

            regionalTendencies[region] = Mathf.Clamp(
                regionalTendencies[region] + amount,
                minTendency,
                maxTendency
            );

            AddTendency(amount * 0.1f);
        }

        string GetCurrentRegion() => "Default";
    }
}