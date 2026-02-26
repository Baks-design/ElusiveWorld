using UnityEngine;

namespace ElusiveWorld.Core.Character
{
    public enum CrouchInput { None, Toggle }
    public enum Stance { Stand, Crouch, Slide }
    
    public struct CharacterInput
    {
        public Quaternion Rotation;
        public Vector2 Move;
        public bool Sprint;
        public bool Jump;
        public bool JumpSustain;
        public CrouchInput Crouch;
    }
}
