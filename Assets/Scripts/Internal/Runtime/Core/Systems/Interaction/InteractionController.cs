using Assets.Scripts.Internal.Runtime.Core.App.Input;
using Assets.Scripts.Internal.Runtime.Core.Behaviours.Player.Movement;
using UnityEngine;

namespace Assets.Scripts.Internal.Runtime.Core.Systems.Interaction
{
    public class InteractionController : PlayerComponent
    {
        [Header("Data")]
        [SerializeField] InputReader input;
        [SerializeField] InteractionData interactionData;
        [Header("Reference")]
        [SerializeField] InteractionUI interactionUI;
        [Header("Ray Settings")]
        [SerializeField] float rayDistance = 0f;
        [SerializeField] float raySphereRadius = 0f;
        [SerializeField] LayerMask interactableLayer = ~0;
        Camera cam;
        bool interacting;
        float holdTimer = 0f;

        void OnEnable()
        {
            input.OnInteractPressed += OnInteractPressed;
            input.OnInteractReleased += OnInteractReleased;
        }

        void Start() => cam = Camera.main;

        void Update()
        {
            CheckForInteractable();
            CheckForInteractableInput();
        }

        void OnDisable()
        {
            input.OnInteractPressed -= OnInteractPressed;
            input.OnInteractReleased -= OnInteractReleased;
        }

        void OnInteractPressed()
        {
            interacting = true;
            holdTimer = 0f;
        }

        void OnInteractReleased()
        {
            interacting = false;
            holdTimer = 0f;
            interactionUI.UpdateChargeProgress(0f);
        }

        void CheckForInteractable()
        {
            var ray = new Ray(cam.transform.position, cam.transform.forward);
            var hitSomething = Physics.SphereCast(ray, raySphereRadius, out var hitInfo, rayDistance, interactableLayer);
                        
            if (hitSomething)
            {
                if (hitInfo.transform.TryGetComponent<InteractableBase>(out var interactable))
                {
                    if (interactionData.IsEmpty())
                    {
                        interactionData.Interactable = interactable;
                        //interactionUI.SetTooltipActiveState(hitSomething);
                        interactionUI.SetToolTip(interactable.TooltipMessage);
                    }
                    else
                    {
                        if (!interactionData.IsSameInteractable(interactable))
                        {
                            interactionData.Interactable = interactable;
                            interactionUI.SetToolTip(interactable.TooltipMessage);
                        }
                    }
                }
            }
            else
            {
                interactionUI.ResetUI();
                interactionData.ResetData();
            }

            //Debug.DrawRay(ray.origin, ray.direction * rayDistance, hitSomething ? Color.green : Color.red);
        }

        void CheckForInteractableInput()
        {
            if (interactionData.IsEmpty()) return;

            if (interacting)
            {
                if (!interactionData.Interactable.IsInteractable) return;

                if (interactionData.Interactable.HoldInteract)
                {
                    holdTimer += Time.deltaTime;

                    var heldPercent = holdTimer / interactionData.Interactable.HoldDuration;
                    interactionUI.UpdateChargeProgress(heldPercent);

                    if (heldPercent > 1f)
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
