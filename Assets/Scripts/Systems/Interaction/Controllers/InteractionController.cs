using ElusiveWorld.Core.Assets.Scripts.Behaviours.Player.Movement;
using ElusiveWorld.Core.Assets.Scripts.Systems.Input;
using ElusiveWorld.Core.Assets.Scripts.Systems.Interaction.Bases;
using ElusiveWorld.Core.Assets.Scripts.Systems.Interaction.Components;
using ElusiveWorld.Core.Assets.Scripts.Systems.Interaction.Data;
using ElusiveWorld.Core.Assets.Scripts.Utils.Services;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Interaction.Controllers
{
    public class InteractionController : PlayerComponent //TODO: Add get Item from Env
    {
        [Header("Data")]
        [SerializeField] InteractionData interactionData;
        [Header("Reference")]
        [SerializeField] InteractionUI interactionUI;
        [Header("Ray Settings")]
        [SerializeField] float rayDistance = 0f;
        [SerializeField] float raySphereRadius = 0f;
        [SerializeField] LayerMask interactableLayer = ~0;
        InputManager input;
        Camera cam;
        bool interacting;
        float holdTimer = 0f;

        void Start()
        {
            cam = Camera.main;
            input = IServiceLocator.Default.GetService<InputManager>();
            input.OnInteractPressed += OnInteractPressed;
            input.OnInteractReleased += OnInteractReleased;
        }

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
