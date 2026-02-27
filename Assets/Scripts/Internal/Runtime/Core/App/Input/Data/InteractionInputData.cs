using UnityEngine;

namespace VHS
{
    [CreateAssetMenu(menuName = "InteractionSystem/InputData")]
    public class InteractionInputData : ScriptableObject
    {
        bool isInteracted;

        public bool IsInteracted
        {
            get => isInteracted;
            set => isInteracted = value;
        }

        public void ResetInput() => isInteracted = false;
    }
}
