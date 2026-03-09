using ElusiveWorld.Core.Assets.Scripts.Systems.Interaction.Bases;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Interaction.Data
{    
    [CreateAssetMenu(fileName = "Interaction Data", menuName = "Data/Systems/Interaction/InteractionData")]
    public class InteractionData : ScriptableObject
    {
        public InteractableBase Interactable { get; set; }

        public void Interact()
        {
            Interactable.OnInteract();
            ResetData();
        }

        public bool IsSameInteractable(InteractableBase newInteractable) => Interactable == newInteractable;

        public bool IsEmpty() => Interactable == null;

        public void ResetData() => Interactable = null;
    }
}
