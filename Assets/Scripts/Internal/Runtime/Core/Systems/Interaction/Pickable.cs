using UnityEngine;

namespace VHS
{
    [RequireComponent(typeof(Rigidbody))]
    public class Pickable : MonoBehaviour, IPickable
    {
        Rigidbody rigid;

        public Rigidbody Rigid { get => rigid; set => rigid = value; }
        public bool Picked { get; set; }

        void Start() => rigid = GetComponent<Rigidbody>();

        public void OnHold() { }

        public void OnPickUp() { }

        public void OnRelease() { }
    }
}
