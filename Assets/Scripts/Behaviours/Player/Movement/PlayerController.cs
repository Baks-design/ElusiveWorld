using Assets.Scripts.Internal.Runtime.Core.Utils.Services;
using UnityEngine;

namespace Assets.Scripts.Internal.Runtime.Core.Behaviours.Player.Movement
{
    public class PlayerController : MonoBehaviour, IService
    {
        PlayerComponent[] playerComponents;

        void Awake()
        {
            IServiceLocator.Default.TryRegisterService(this);
            InitPlayerComponents();
        }

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
