using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Events;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Services;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Updates.Variable;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Updates.Variable.Interfaces;
using ElusiveWorld.Core.Assets.Scripts.Systems.Input;
using ElusiveWorld.Core.Assets.Scripts.Systems.Interaction.Bases;
using ElusiveWorld.Core.Assets.Scripts.Systems.Interaction.Data;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Interaction.Controllers
{
    public class InteractionController : MonoBehaviour, IUpdate
    {
        [Header("Data")]
        [SerializeField] InteractionData interactionData;
        [Header("Ray Settings")]
        [SerializeField] float rayDistance = 0f;
        [SerializeField] float raySphereRadius = 0f;
        [SerializeField] LayerMask interactableLayer = ~0;
        InputManager input;
        Camera cam;
        Ray ray;
        bool interacting;
        bool hitSomething;
        float holdTimer = 0f;

        void Awake()
        {
            cam = Camera.main;
            input = IServiceLocator.Default.GetService<InputManager>();
        }

        void OnEnable()
        {
            InputSubscribe();
            UpdateManager.RegisterUpdate(this);
        }

        void IUpdate.Update()
        {
            CheckForInteractable();
            CheckForInteractableInput();
        }

        void OnDisable()
        {
            InputUnsubscribe();
            UpdateManager.UnregisterUpdate(this);
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = hitSomething ? Color.red : Color.green;
            Gizmos.DrawRay(ray.origin, ray.direction * rayDistance);
        }

        void InputSubscribe()
        {
            input.OnInteractPressed += OnInteractPressed;
            input.OnInteractReleased += OnInteractReleased;
        }

        void InputUnsubscribe()
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
            EventBus<InteractEvent>.Raise(new InteractEvent { updateProgress = 0f });
        }

        void CheckForInteractable()
        {
            ray = new Ray(cam.transform.position, cam.transform.forward);
            hitSomething = Physics.SphereCast(
                ray, raySphereRadius, out var hitInfo, rayDistance, interactableLayer, QueryTriggerInteraction.Ignore);
            if (hitSomething)
            {
                if (hitInfo.transform.TryGetComponent<InteractableBase>(out var interactable))
                {
                    if (interactionData.IsEmpty())
                    {
                        interactionData.Interactable = interactable;
                        EventBus<InteractEvent>.Raise(new InteractEvent { setTooltip = interactable.TooltipMessage });
                    }
                    else
                    {
                        if (!interactionData.IsSameInteractable(interactable))
                        {
                            interactionData.Interactable = interactable;
                            EventBus<InteractEvent>.Raise(new InteractEvent { setTooltip = interactable.TooltipMessage });
                        }
                    }
                }
            }
            else
            {
                EventBus<InteractEvent>.Raise(new InteractEvent { resetUI = true });
                interactionData.ResetData();
            }
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
                    EventBus<InteractEvent>.Raise(new InteractEvent { updateProgress = heldPercent});

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
