using ElusiveWorld.Core.Assets.Scripts.Systems.Interaction.Types;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Interaction.Data
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