using UnityEngine;
using System.Collections.Generic;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Services;
using ElusiveWorld.Core.Assets.Scripts.Systems.Input;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Characters
{
    public class CharactersController : MonoBehaviour
    {
        [SerializeField] Transform modelParent;
        [SerializeField] CharacterData defaultCharacter;
        readonly List<CharacterData> collectedCharacters = new();
        CharacterData currentCharacterData;
        GameObject currentModel;
        Animator currentAnimator;
        InputManager input;
        int currentCharacterIndex;

        void OnEnable()
        {
            input = IServiceLocator.Default.GetService<InputManager>();
            if (input == null) return;
            input.OnShootPressed += NextCharacter;
            input.OnReloadPressed += PreviousCharacter;
        }

        void Start()
        {
            if (defaultCharacter == null) return;

            collectedCharacters.Add(defaultCharacter);
            SwitchCharacter(0);
        }

        void OnDisable()
        {
            if (input == null) return;
            input.OnShootPressed -= NextCharacter;
            input.OnReloadPressed -= PreviousCharacter;
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
            currentCharacterIndex = index;
            currentCharacterData = collectedCharacters[index];

            if (currentModel != null) Destroy(currentModel);

            currentModel = Instantiate(currentCharacterData.characterModel, modelParent);
            currentModel.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            SetupAnimator();
            ApplyVisuals();

            Debug.Log($"Trocou para: {currentCharacterData.characterName}");

            OnCharacterChanged();
        }

        void OnCharacterChanged() { }

        void SetupAnimator()
        {
            currentAnimator = currentModel.GetComponent<Animator>();
            if (currentAnimator != null && currentCharacterData.animator != null)
                currentAnimator.runtimeAnimatorController = currentCharacterData.animator;
        }

        void ApplyVisuals()
        {
            var renderers = currentModel.GetComponentsInChildren<Renderer>();
            foreach (var r in renderers) r.material.color = currentCharacterData.characterColor;
        }

        public void CollectCharacter(CharacterData newCharacter)
        {
            if (collectedCharacters.Contains(newCharacter))
            {
                Debug.Log("Você já tem esse personagem!");
                return;
            }

            collectedCharacters.Add(newCharacter);
            Debug.Log($"Coletou: {newCharacter.characterName}! Total: {collectedCharacters.Count}");
            if (collectedCharacters.Count == 2) SwitchCharacter(1);
        }
    }
}