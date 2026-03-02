using UnityEngine;

namespace Assets.Scripts.Internal.Runtime.Core.Systems.Interaction
{    
    public interface IPickable
    {
        Rigidbody Rigid {get;set;}

        void OnPickUp();
        void OnHold();
        void OnRelease();
    }
}
