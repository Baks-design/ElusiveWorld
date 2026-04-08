using UnityEngine;
using System.Collections.Generic;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Services;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Updates.Variable;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Updates.Variable.Interfaces;
using ElusiveWorld.Core.Assets.Scripts.Systems.Input;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Characters
{
    [RequireComponent(typeof(CharacterController))]
    public class CharactersController : MonoBehaviour, IUpdate
    {
        [SerializeField] Transform modelParent;
        [SerializeField] CharacterData defaultCharacter;
        readonly List<CharacterData> collectedCharacters = new();
        int currentCharacterIndex = 0;
        CharacterData currentCharacterData;
        GameObject currentModel;
        Animator currentAnimator;
        InputManager input;

        void OnEnable()
        {
            UpdateManager.RegisterUpdate(this);
            input = IServiceLocator.Default.GetService<InputManager>();
            input.OnShootPressed += NextCharacter;
            input.OnReloadPressed += PreviousCharacter;
        }

        void Start()
        {
            if (defaultCharacter != null)
            {
                collectedCharacters.Add(defaultCharacter);
                SwitchCharacter(0);
            }
        }

        void IUpdate.Update() { }

        void OnDisable()
        {
            UpdateManager.UnregisterUpdate(this);
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

        void SwitchCharacter(int index)
        {
            currentCharacterIndex = index;
            currentCharacterData = collectedCharacters[currentCharacterIndex];

            if (currentModel != null) Destroy(currentModel);

            currentModel = Instantiate(currentCharacterData.characterModel, modelParent);
            currentModel.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            currentAnimator = currentModel.GetComponent<Animator>();
            if (currentAnimator != null && currentCharacterData.animator != null)
                currentAnimator.runtimeAnimatorController = currentCharacterData.animator;

            var renderers = currentModel.GetComponentsInChildren<Renderer>();
            foreach (var rend in renderers)
                rend.material.color = currentCharacterData.characterColor;

            Debug.Log($"Trocou para: {currentCharacterData.characterName}");

            OnCharacterChanged();
        }

        void OnCharacterChanged() { }
    }
}