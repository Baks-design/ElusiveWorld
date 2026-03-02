using UnityEngine;

namespace Assets.Scripts.Internal.Runtime.Core.Systems.Interaction
{
    public class InteractableBase : MonoBehaviour, IInteractable
    {
        [field: SerializeField] public float HoldDuration { get; private set; } = 1f;
        [field: SerializeField] public bool HoldInteract { get; private set; } = true;
        [field: SerializeField] public bool MultipleUse { get; private set; } = false;
        [field: SerializeField] public bool IsInteractable { get; private set; } = true;

        public virtual void OnInteract() => Debug.Log($"INTERACTED: {gameObject.name}");
    }
}
