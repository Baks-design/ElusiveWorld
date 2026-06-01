using UnityEngine.UI;
using UnityEngine;
using TMPro;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Events;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Updates.Variable;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Updates.Variable.Interfaces;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Interaction.Components
{
    [RequireComponent(typeof(RectTransform))]
    public class InteractionUI : MonoBehaviour, IUpdate
    {
        [SerializeField] Image holdProgressIMG;
        [SerializeField] Image tooltipBG;
        [SerializeField] RectTransform canvasTransform;
        [SerializeField] TextMeshProUGUI interactableTooltip;
        EventBinding<InteractEvent> interactEventBinding;
        bool resetUI = false;
        string setTooltip = "";
        float updateProgress = 0f;

        public bool IsTooltipActive => interactableTooltip.gameObject.activeSelf;

        void OnEnable()
        {
            UpdateManager.RegisterUpdate(this);

            interactEventBinding = new EventBinding<InteractEvent>(HandleInteractEvent);
            EventBus<InteractEvent>.Register(interactEventBinding);
        }

        void OnDisable()
        {
            EventBus<InteractEvent>.Deregister(interactEventBinding);

            UpdateManager.UnregisterUpdate(this);
        }

        void HandleInteractEvent(InteractEvent interactEvent)
        {
            resetUI = interactEvent.resetUI;
            setTooltip = interactEvent.setTooltip;
            updateProgress += interactEvent.updateProgress;
        }

        void Start()
        {
            canvasTransform = GetComponent<RectTransform>();
            interactableTooltip = GetComponentInChildren<TextMeshProUGUI>();
            ResetUI();
        }

        void IUpdate.Update()
        {
            UpdateChargeProgress();
            SetToolTip();
            UpdateResetUI();
        }

        public void SetTooltipActiveState(bool state)
        {
            interactableTooltip.gameObject.SetActive(state);
            holdProgressIMG.gameObject.SetActive(state);
            tooltipBG.gameObject.SetActive(state);
        }

        void SetToolTip() => interactableTooltip.SetText(setTooltip);

        void UpdateChargeProgress() => holdProgressIMG.fillAmount = updateProgress;

        void UpdateResetUI()
        {
            if (!resetUI) return;
            ResetUI();
        }

        void ResetUI()
        {
            holdProgressIMG.fillAmount = 0f;
            interactableTooltip.SetText("");
        }

        public void LookAtPlayer(Transform player) => canvasTransform.LookAt(player, Vector3.up);

        public void UnparentToltip() => canvasTransform.SetParent(null);
    }
}