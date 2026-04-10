using UnityEngine;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Services;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Updates.Variable;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Updates.Variable.Interfaces;
using ElusiveWorld.Core.Assets.Scripts.Systems.Input;
using Unity.Cinemachine;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Characters
{
    public class CharactersLook : MonoBehaviour, ILateUpdate
    {
        [SerializeField] CinemachineCamera cam;
        [SerializeField] Transform camTransform;
        [SerializeField] Transform yawTransform;
        [SerializeField] Transform pitchTransform;
        [SerializeField] LookSettings settings;
        CameraRotation rotation;
        CameraSwaying swaying;
        CameraZoom zoom;
        CameraBreathing breathing;
        InputManager input;
        float dt;

        void Awake()
        {
            rotation = new(settings, yawTransform, pitchTransform);
            swaying = new(settings, camTransform);
            zoom = new(settings, cam);
            breathing = new(settings, camTransform);
        }

        void OnEnable()
        {
            UpdateManager.RegisterLateUpdate(this);

            input = IServiceLocator.Default.GetService<InputManager>();
            if (input == null) return;
            input.OnZoomPressed += OnZoom;
            input.OnZoomReleased += OnZoom;
        }

        void OnDisable()
        {
            UpdateManager.UnregisterLateUpdate(this);

            if (input == null) return;
            input.OnZoomPressed -= OnZoom;
            input.OnZoomReleased -= OnZoom;
        }

        void ILateUpdate.LateUpdate(float dt)
        {
            this.dt = dt;
            
            if (input == null) return;
            rotation.Update(input, dt);
            breathing.Update(dt);
        }

        void OnZoom() => zoom.ChangeFOV(this, dt);

        public void ChangeRunFOV(bool returning) =>
            zoom.ChangeRunFOV(this, returning, dt);

        public void HandleSway(Vector3 inputVector, float rawXInput) =>
            swaying.SwayPlayer(inputVector, rawXInput, dt);
    }
}