using UnityEngine;

namespace ElusiveWorld.Core.Character
{
    public struct CharacterState
    {
        public bool Grounded;
        public Stance Stance;
        public Vector3 Velocity;
        public Vector3 Acceleration;
    }
}
