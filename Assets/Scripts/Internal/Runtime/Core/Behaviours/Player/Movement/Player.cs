using UnityEngine;

namespace Assets.Scripts.Internal.Runtime.Core.Behaviours.Player.Movement
{
    public class Player : MonoBehaviour
    {
        PlayerComponent[] playerComponents;

        void Awake() => InitPlayerComponents();

        public void InitPlayerComponents()
        {
            playerComponents = GetComponentsInChildren<PlayerComponent>();
            foreach (var baseComp in playerComponents)
                baseComp.InitPlayerReference(this);
        }
        
        public T FetchComponent<T>() where T : PlayerComponent
        {
            var temp = default(T);
            foreach (var playerComponent in playerComponents)
                if (playerComponent is T)
                    temp = playerComponent as T;
            return temp;
        }
    }
}
