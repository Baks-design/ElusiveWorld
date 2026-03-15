using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Services;
using ElusiveWorld.Core.Assets.Scripts.Systems.Tendency.Interfaces;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Tendency.Effects
{
    public abstract class TendencyEffect : MonoBehaviour, ITendencyEffect
    {
        [SerializeField] protected TendencyState requiredState;

        public TendencyState RequiredState => requiredState;

        protected virtual void Start()
            => IServiceLocator.Default.GetService<TendencyManager>().RegisterEffect(this);

        protected virtual void OnDestroy()
            => IServiceLocator.Default.GetService<TendencyManager>().UnregisterEffect(this);

        public abstract void Apply();

        public abstract void Remove();
    }
}