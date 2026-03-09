using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace Assets.Scripts.Internal.Runtime.Core.Systems.Interaction
{
    [RequireComponent(typeof(RectTransform))]
    public class InteractionUI : MonoBehaviour
    {
        [SerializeField] Image holdProgressIMG;
        [SerializeField] Image tooltipBG;
        [SerializeField] RectTransform canvasTransform;
        [SerializeField] TextMeshProUGUI interactableTooltip;

        public bool IsTooltipActive => interactableTooltip.gameObject.activeSelf;

        void Start()
        {
            canvasTransform = GetComponent<RectTransform>();
            interactableTooltip = GetComponentInChildren<TextMeshProUGUI>();
            ResetUI();
        }

        public void SetTooltipActiveState(bool state)
        {
            interactableTooltip.gameObject.SetActive(state);
            holdProgressIMG.gameObject.SetActive(state);
            tooltipBG.gameObject.SetActive(state);
        }

        public void SetToolTip(string tooltip) => interactableTooltip.SetText(tooltip);

        public void UpdateChargeProgress(float progress) => holdProgressIMG.fillAmount = progress;

        public void LookAtPlayer(Transform player) => canvasTransform.LookAt(player, Vector3.up);

        public void UnparentToltip() => canvasTransform.SetParent(null);

        public void ResetUI()
        {
            holdProgressIMG.fillAmount = 0f;
            interactableTooltip.SetText("");
        }
    }
}