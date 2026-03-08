using UnityEngine;

namespace Assets.Scripts.Internal.Runtime.Core.Systems.WorldTendency
{
    public abstract class TendencyEffect : MonoBehaviour, ITendencyEffect
    {
        [SerializeField] protected TendencyState requiredState;

        public TendencyState RequiredState => requiredState;

        protected virtual void Start() => TendencyManager.Instance.RegisterEffect(this);

        protected virtual void OnDestroy() => TendencyManager.Instance.UnregisterEffect(this);

        public abstract void Apply();

        public abstract void Remove();
    }
}