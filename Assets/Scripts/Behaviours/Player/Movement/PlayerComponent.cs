using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Player.Movement
{
    public class PlayerComponent : MonoBehaviour
    {
        protected PlayerController Player { get; private set; }

        public void InitPlayerReference(PlayerController player) => Player = player;
    }
}
