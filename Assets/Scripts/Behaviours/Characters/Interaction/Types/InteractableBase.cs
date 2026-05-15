using ElusiveWorld.Core.Assets.Scripts.Systems.Interaction.Interfaces;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Interaction.Bases
{
    public class InteractableBase : MonoBehaviour, IInteractable
    {
        [field: SerializeField] public float HoldDuration { get; private set; } = 1f;
        [field: SerializeField] public bool HoldInteract { get; private set; } = true;
        [field: SerializeField] public bool MultipleUse { get; private set; } = false;
        [field: SerializeField] public bool IsInteractable { get; private set; } = true;
        [field: SerializeField] public string TooltipMessage { get; private set; } = "Interact";

        public virtual void OnInteract() { }
    }
}
