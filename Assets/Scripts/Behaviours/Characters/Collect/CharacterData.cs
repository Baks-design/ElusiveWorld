using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Characters
{
    [CreateAssetMenu(fileName = "NewCharacter", menuName = "Data/Characters/Character Data")]
    public class CharacterData : ScriptableObject
    {
        public string characterName;
        public GameObject characterModel;
        public RuntimeAnimatorController animator;
        public float moveSpeed = 5f;
        public float jumpForce = 10f;
        public Color characterColor = Color.white;
        public AudioClip collectSound;
        public GameObject collectEffect;
    }
}