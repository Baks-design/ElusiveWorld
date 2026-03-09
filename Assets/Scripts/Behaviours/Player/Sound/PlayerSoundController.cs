using ElusiveWorld.Core.Assets.Scripts.Systems.Audio.Components;
using ElusiveWorld.Core.Assets.Scripts.Systems.Audio.Managers;
using ElusiveWorld.Core.Assets.Scripts.Utils.Services;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Player.Sound
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
            soundBuilder = IServiceLocator.Default.GetService<SoundManager>().CreateSoundBuilder();

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