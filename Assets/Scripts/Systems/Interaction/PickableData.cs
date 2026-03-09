using UnityEngine;

namespace Assets.Scripts.Internal.Runtime.Core.Systems.Interaction
{
    [CreateAssetMenu(fileName = "PickableData", menuName = "Data/Systems/Interaction/PickableData")]
    public class PickableData : ScriptableObject
    {
        public Pickable PickableItem { get; set; }

        public bool IsEmpty() => PickableItem == null;

        public bool IsSamePickable(Pickable pickable) => PickableItem == pickable;

        public void Pick() => PickableItem.OnPickUp();

        public void Hold() => PickableItem.OnHold();

        public void Release() => PickableItem.OnRelease();

        public void ResetData() => PickableItem = null;
    }
}