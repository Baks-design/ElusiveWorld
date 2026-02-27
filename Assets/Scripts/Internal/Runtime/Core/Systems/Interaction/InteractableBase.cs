using UnityEngine;

namespace VHS
{
    public class InteractableBase : MonoBehaviour, IInteractable
    {
        [SerializeField] float holdDuration = 1f;
        [SerializeField] bool holdInteract = true;
        [SerializeField] bool multipleUse = false;
        [SerializeField] bool isInteractable = true;

        public float HoldDuration => holdDuration;
        public bool HoldInteract => holdInteract;
        public bool MultipleUse => multipleUse;
        public bool IsInteractable => isInteractable;

        public virtual void OnInteract() => Debug.Log($"INTERACTED: {gameObject.name}");
    }
}
