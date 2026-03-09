using UnityEngine;

namespace Assets.Scripts.Internal.Runtime.Core.Systems.Interaction
{
    [RequireComponent(typeof(Rigidbody))]
    public class Pickable : MonoBehaviour, IPickable
    {
        [field: SerializeField] public Rigidbody Rigid { get; set; }

        void Start() => Rigid = GetComponent<Rigidbody>();

        public void OnHold() { }
        public void OnPickUp() { }
        public void OnRelease() { }
    }
}
