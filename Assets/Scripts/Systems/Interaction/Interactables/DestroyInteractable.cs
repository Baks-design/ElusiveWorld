using ElusiveWorld.Core.Assets.Scripts.Systems.Interaction.Bases;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Interaction.Interactables
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
