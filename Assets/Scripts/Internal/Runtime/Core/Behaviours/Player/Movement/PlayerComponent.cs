using UnityEngine;

namespace Assets.Scripts.Internal.Runtime.Core.Behaviours.Player.Movement
{
    public class PlayerComponent : MonoBehaviour
    {
        protected Player Player { get; private set; }

        public void InitPlayerReference(Player player) => Player = player;
    }
}
