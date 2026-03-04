using Assets.Scripts.Internal.Runtime.Core.Systems.Interaction;

namespace Assets.Scripts.Internal.Runtime.Core.Behaviours.Interactables
{
    public class DestroyInteractable : InteractableBase
    {
        public override void OnInteract()
        {
            base.OnInteract();
            Destroy(gameObject);
        }
    }
}
