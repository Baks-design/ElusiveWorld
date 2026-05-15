using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Characters
{
    [CreateAssetMenu(fileName = "CharacterData", menuName = "Data/Behaviours/Characters/Character Data", order = 0)]
    public class CharacterData : ScriptableObject
    {
        [Header("Refs")]
        public string characterName;
        public GameObject characterModel;
        // public RuntimeAnimatorController animator;
        // public AudioClip collectSound;
        // public GameObject collectEffect;
        [Header("Stats")]
        public Color characterColor = Color.white;
        [SerializeField] LookSettings lookSettings;
        [SerializeField] MovementSettings movementSettings;
    }
}