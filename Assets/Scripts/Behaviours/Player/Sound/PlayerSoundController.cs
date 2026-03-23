using ElusiveWorld.Core.Assets.Scripts.Systems.Audio.Components;
using ElusiveWorld.Core.Assets.Scripts.Systems.Audio.Managers;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Services;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Updates.Variable;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Updates.Variable.Interfaces;
using UnityEngine;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Player.Sound
{
    public class PlayerSoundController : MonoBehaviour, IUpdate
    {
        [Header("Components")]
        [SerializeField] Transform groundCheckPoint;
        [Header("Classes")]
        [SerializeField] FootstepsSounds footstepsSounds;
        SoundBuilder soundBuilder;

        void OnEnable() => UpdateManager.RegisterUpdate(this);

        void Start()
        {
            GetComponents();
            Initialize();
        }

        void IUpdate.Update() => footstepsSounds.Update();

        void OnDisable() => UpdateManager.UnregisterUpdate(this);

        void OnDrawGizmosSelected() => footstepsSounds.DrawGizmos();

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
    }
}