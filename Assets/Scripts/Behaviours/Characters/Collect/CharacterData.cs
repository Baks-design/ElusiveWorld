using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Characters
{
    [CreateAssetMenu(fileName = "CharacterData", menuName = "Data/Characters/Character Data")]
    public class CharacterData : ScriptableObject
    {
        [Header("Stats")]
        public float moveSpeed = 5f;
        public float jumpForce = 10f;
        public Color characterColor = Color.white;
        
        [Header("Refs")]
        public string characterName;
        public GameObject characterModel;
        public RuntimeAnimatorController animator;
        public AudioClip collectSound;
        public GameObject collectEffect;
    }
}