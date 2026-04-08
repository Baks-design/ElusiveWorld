using ElusiveWorld.Core.Assets.Scripts.Utils.Extensions;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Characters
{
    public class CollectableCharacter : MonoBehaviour
    {
        [SerializeField] LayerMask characterLayer;
        [SerializeField] CharacterData characterData;

        void OnTriggerEnter(Collider other)
        {
            if (characterLayer.ContainsLayer(other.gameObject))
            {
                if (other.TryGetComponent<CharactersController>(out var player))
                    player.CollectCharacter(characterData);

                Destroy(gameObject);
            }
        }
    }
}