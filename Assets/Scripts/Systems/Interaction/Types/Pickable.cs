using ElusiveWorld.Core.Assets.Scripts.Systems.Interaction.Interfaces;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Systems.Interaction.Types
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
