using UnityEngine;

namespace Assets.Scripts.Internal.Runtime.Core.Systems.Interaction
{
    [RequireComponent(typeof(Rigidbody))]
    public class Pickable : MonoBehaviour, IPickable
    {
        public Rigidbody Rigid { get; set; }

        void Awake() => Rigid = GetComponent<Rigidbody>();

        public void OnHold() { }
        public void OnPickUp() { }
        public void OnRelease() { }
    }
}
