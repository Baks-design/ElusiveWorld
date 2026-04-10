using UnityEngine;
using ElusiveWorld.Core.Assets.Scripts.Systems.Audio.Data;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Characters
{
    public class CharactersEffects : MonoBehaviour 
    {
        [SerializeField] CharacterController controller;
        [SerializeField] Transform spawn;
        [SerializeField] SoundLibraryObject footstepsLibrary;
    }
}