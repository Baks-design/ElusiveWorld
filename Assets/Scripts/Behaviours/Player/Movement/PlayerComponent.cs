using UnityEngine;

namespace Assets.Scripts.Internal.Runtime.Core.Behaviours.Player.Movement
{
    public class PlayerComponent : MonoBehaviour
    {
        protected PlayerController Player { get; private set; }

        public void InitPlayerReference(PlayerController player) => Player = player;
    }
}
