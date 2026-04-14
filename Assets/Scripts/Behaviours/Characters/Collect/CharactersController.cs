using UnityEngine;
using System.Collections.Generic;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Services;
using ElusiveWorld.Core.Assets.Scripts.Systems.Input;
using System;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Characters
{
    public class CharactersController : MonoBehaviour
    {
        [SerializeField] Transform modelParent;
        [SerializeField] CharacterData defaultCharacter;
        readonly List<CharacterData> collectedCharacters = new();
        readonly int colorID = Shader.PropertyToID("_Color");
        CharacterData currentCharacterData;
        GameObject currentModel;
        Animator currentAnimator;
        InputManager input;
        int currentCharacterIndex;

        public event Action<CharacterData> OnCharacterChanged = delegate { };

        void Awake() => input = IServiceLocator.Default.GetService<InputManager>();

        void OnEnable() => InputSubscribe();

        void Start()
        {
            AddInitializeCharacter();
            SwitchCharacter(0);
        }

        void OnDisable() => InputUnsubscribe();

        void InputSubscribe()
        {
            if (input == null) return;
            input.OnNextCharacterPressed += NextCharacter;
            input.OnPreviousCharacterPressed += PreviousCharacter;
        }

        void InputUnsubscribe()
        {
            if (input == null) return;
            input.OnNextCharacterPressed -= NextCharacter;
            input.OnPreviousCharacterPressed -= PreviousCharacter;
        }

        void AddInitializeCharacter()
        {
            if (defaultCharacter == null) return;
            if (!collectedCharacters.Contains(defaultCharacter))
                collectedCharacters.Add(defaultCharacter);
        }

        void NextCharacter()
        {
            if (collectedCharacters.Count <= 1) return;

            currentCharacterIndex = (currentCharacterIndex + 1) % collectedCharacters.Count;
            SwitchCharacter(currentCharacterIndex);
        }

        void PreviousCharacter()
        {
            if (collectedCharacters.Count <= 1) return;

            currentCharacterIndex = (currentCharacterIndex - 1 + collectedCharacters.Count) % collectedCharacters.Count;
            SwitchCharacter(currentCharacterIndex);
        }

        void SwitchCharacter(int index)
        {
            if (collectedCharacters.Count == 0 || index < 0 || index >= collectedCharacters.Count) return;

            currentCharacterIndex = index;
            currentCharacterData = collectedCharacters[index];

            var cache = new Dictionary<CharacterData, GameObject>();
            if (!cache.TryGetValue(currentCharacterData, out currentModel))
            {
                currentModel = Instantiate(currentCharacterData.characterModel, modelParent);
                cache[currentCharacterData] = currentModel;
                currentModel.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            }

            SetupAnimator();
            ApplyVisuals();

            OnCharacterChanged.Invoke(currentCharacterData);

            //Debug.Log($"Trocou para: {currentCharacterData.characterName}");
        }

        void SetupAnimator()
        {
            currentAnimator = currentModel.GetComponent<Animator>();
            if (currentAnimator != null && currentCharacterData.animator != null)
                currentAnimator.runtimeAnimatorController = currentCharacterData.animator;
        }

        void ApplyVisuals()
        {
            var renderers = currentModel.GetComponentsInChildren<Renderer>();
            var block = new MaterialPropertyBlock();
            foreach (var r in renderers)
            {
                r.GetPropertyBlock(block);
                block.SetColor(colorID, currentCharacterData.characterColor);
                r.SetPropertyBlock(block);
            }
        }

        public void CollectCharacter(CharacterData newCharacter)
        {
            if (collectedCharacters.Contains(newCharacter))
            {
                //Debug.Log("Você já tem esse personagem!");
                return;
            }

            collectedCharacters.Add(newCharacter);
            //Debug.Log($"Coletou: {newCharacter.characterName}! Total: {collectedCharacters.Count}");
            if (collectedCharacters.Count == 2 && currentCharacterIndex == 0)
                SwitchCharacter(1);
        }
    }
}