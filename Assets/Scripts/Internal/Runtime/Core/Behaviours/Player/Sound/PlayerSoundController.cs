using Assets.Scripts.Internal.Runtime.Core.Systems.Audio;
using UnityEngine;

namespace Assets.Scripts.Internal.Runtime.Core.Behaviours.Player.Sound
{
    public class PlayerSoundController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] Transform groundCheckPoint;
        [Header("Classes")]
        [SerializeField] FootstepsSounds footstepsSounds;
        SoundBuilder soundBuilder;

        void Start()
        {
            GetComponents();
            Initialize();
        }

        void GetComponents()
        {
            soundBuilder = SoundManager.Instance.CreateSoundBuilder();

            groundCheckPoint = transform.Find("GroundCheck");
            if (groundCheckPoint == null)
            {
                groundCheckPoint = new GameObject("GroundCheck").transform;
                groundCheckPoint.parent = transform;
                groundCheckPoint.localPosition = Vector3.zero;
            }
        }

        void Initialize() => footstepsSounds.Initialize(groundCheckPoint, soundBuilder);

        void Update() => footstepsSounds.Update();

        void OnDrawGizmosSelected() => footstepsSounds?.DrawGizmos();
    }
}