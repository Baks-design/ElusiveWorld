using System.Collections;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Characters
{
    public class CharactersFlags
    {
        public Vector2 inputVector;
        public Vector2 smoothInputVector;
        public Vector3 finalMoveDir;
        public Vector3 smoothFinalMoveDir;
        public Vector3 finalMoveVector;
        public Vector3 horizontalVelocity;
        public Vector3 slideVelocity;
        public float verticalVelocity;
        public float currentSpeed;
        public float smoothCurrentSpeed;
        public float finalSmoothCurrentSpeed;
        public float walkRunSpeedDifference;
        public float finalRayLength;
        public bool hitWall;
        public bool isCrouching;
        public bool isSliding;
        public bool isRunning;
        public bool isGrounded;
        public bool isHitWall;
        public bool previouslyGrounded;
        public float initHeight;
        public float crouchHeight;
        public float slideHeight;
        public Vector3 initCenter;
        public Vector3 crouchCenter;
        public Vector3 slideCenter;
        public float initCamHeight;
        public float crouchCamHeight;
        public float slideCamHeight;
        public float crouchStandHeightDifference;
        public float slideStandHeightDifference;
        public bool duringCrouchAnimation;
        public bool duringRunAnimation;
        public bool duringSlideAnimation;
        public float inAirTimer;
        public RaycastHit hitInfo;
        public IEnumerator landRoutine;
        public Coroutine crouchRoutine;
    }
}