using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Interaction.Interfaces
{    
    public interface IPickable
    {
        Rigidbody Rigid {get;set;}

        void OnPickUp();
        void OnHold();
        void OnRelease();
    }
}
