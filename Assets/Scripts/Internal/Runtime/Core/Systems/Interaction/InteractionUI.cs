using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace VHS
{
    [RequireComponent(typeof(RectTransform))]
    public class InteractionUI : MonoBehaviour
    {
        [SerializeField] Image holdProgressIMG;
        [SerializeField] Image tooltipBG;
        RectTransform canvasTransform;
        TextMeshProUGUI interactableTooltip;

        public void Init()
        {
            canvasTransform = GetComponent<RectTransform>();
            interactableTooltip = GetComponentInChildren<TextMeshProUGUI>();
        }

        public void SetToolTip(Transform parent, string tooltip, float holdProgress)
        {
            if (parent)
            {
                canvasTransform.position = parent.position;
                canvasTransform.SetParent(parent);
            }

            interactableTooltip.SetText(tooltip);
            holdProgressIMG.fillAmount = holdProgress;
        }

        public void SetTooltipActiveState(bool state)
        {
            interactableTooltip.gameObject.SetActive(state);
            holdProgressIMG.gameObject.SetActive(state);
            tooltipBG.gameObject.SetActive(state);
        }

        public void UpdateChargeProgress(float progress) => holdProgressIMG.fillAmount = progress;

        public void LookAtPlayer(Transform player) => canvasTransform.LookAt(player, Vector3.up);

        public void UnparentToltip() => canvasTransform.SetParent(null);

        public bool IsTooltipActive() => interactableTooltip.gameObject.activeSelf;
    }
}