using UnityEngine;

namespace VHS
{
    [CreateAssetMenu(menuName = "Systems/Interaction/InteractionData")]
    public class InteractionData : ScriptableObject
    {
        InteractableBase interactable;

        public InteractableBase Interactable
        {
            get => interactable;
            set => interactable = value;
        }
        public bool IsEmpty => interactable == null;

        public void Interact()
        {
            interactable.OnInteract();
            ResetData();
        }

        public bool IsSameInteractable(InteractableBase newInteractable) =>
            interactable == newInteractable;

        public void ResetData() => interactable = null;
    }
}
