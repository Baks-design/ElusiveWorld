using UnityEngine;

namespace VHS
{
    public class InteractionController : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] InteractionInputData interactionInputData;
        [SerializeField] InteractionData interactionData;
        [Header("Ray Settings")]
        [SerializeField] float rayDistance = 0f;
        [SerializeField] float raySphereRadius = 0f;
        [SerializeField] LayerMask interactableLayer = ~0;
        Camera cam;
        bool interacting;
        float holdTimer = 0f;

        void Start() => cam = Camera.main;

        void Update()
        {
            CheckForInteractable();
            CheckForInteractableInput();
        }

        void CheckForInteractable()
        {
            var ray = new Ray(cam.transform.position, cam.transform.forward);
            var hitSomething = Physics.SphereCast(ray, raySphereRadius, out var hitInfo, rayDistance, interactableLayer);

            if (hitSomething)
            {
                if (hitInfo.transform.TryGetComponent<InteractableBase>(out var interactable))
                {
                    if (interactionData.IsEmpty)
                        interactionData.Interactable = interactable;
                    else
                    {
                        if (!interactionData.IsSameInteractable(interactable))
                            interactionData.Interactable = interactable;
                    }
                }
            }
            else
                interactionData.ResetData();

            Debug.DrawRay(ray.origin, ray.direction * rayDistance, hitSomething ? Color.green : Color.red);
        }

        void CheckForInteractableInput()
        {
            if (interactionData.IsEmpty) return;

            if (interactionInputData.IsInteracted)
            {
                interacting = true;
                holdTimer = 0f;
            }
            if (interactionInputData.IsInteracted)
            {
                interacting = false;
                holdTimer = 0f;
            }

            if (interacting)
            {
                if (!interactionData.Interactable.IsInteractable) return;

                if (interactionData.Interactable.HoldInteract)
                {
                    holdTimer += Time.deltaTime;
                    if (holdTimer >= interactionData.Interactable.HoldDuration)
                    {
                        interactionData.Interact();
                        interacting = false;
                    }
                }
                else
                {
                    interactionData.Interact();
                    interacting = false;
                }
            }
        }
    }
}
