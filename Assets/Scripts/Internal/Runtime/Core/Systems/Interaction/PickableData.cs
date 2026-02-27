using UnityEngine;

namespace VHS
{
    [CreateAssetMenu(menuName = "Data/PickableData")]
    public class PickableData : ScriptableObject
    {
        Pickable pickable;
        
        public Pickable PickableItem
        {
            get => pickable;
            set => pickable = value;
        }

        public bool IsEmpty()
        {
            if (pickable != null)
                return false;
            else
                return true;
        }

        public bool IsSamePickable(Pickable pickable)
        {
            if (this.pickable == pickable)
                return true;
            else
                return false;
        }

        public void Pick() => pickable.OnPickUp();

        public void Hold() => pickable.OnHold();

        public void Release() => pickable.OnRelease();

        public void ResetData() => pickable = null;
    }
}