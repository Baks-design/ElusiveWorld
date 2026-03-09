using Assets.Scripts.Internal.Runtime.Core.Utils.Services;
using UnityEngine;

namespace Assets.Scripts.Internal.Runtime.Core.Systems.WorldTendency
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