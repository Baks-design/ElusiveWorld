using ElusiveWorld.Core.Assets.Scripts.Utils.Services;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Player.Movement
{
    public class PlayerController : MonoBehaviour, IService
    {
        PlayerComponent[] playerComponents;

        public void Initialize() => InitPlayerComponents();

        public void Dispose() { }

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

        public void MoveToRandomPosition() { } 
    }
}
