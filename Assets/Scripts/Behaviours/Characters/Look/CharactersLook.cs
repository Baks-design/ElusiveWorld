using UnityEngine;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Services;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Updates.Variable;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Updates.Variable.Interfaces;
using ElusiveWorld.Core.Assets.Scripts.Systems.Input;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Characters
{
    public class CharactersLook : MonoBehaviour, ILateUpdate
    {
        [SerializeField] Transform camTransform;
        [SerializeField] Transform yawTranform;
        [SerializeField] Transform pitchTranform;
        [SerializeField] LookSettings settings;
        CameraRotation rotation;
        CameraSwaying swaying;
        CameraZoom zoom;
        CameraBreathing breathing;
        InputManager input;

        void Awake()
        {
            rotation = new(settings, yawTranform, pitchTranform);
            swaying = new(settings, camTransform);
            zoom = new(settings, Camera.main);
            breathing = new(settings, camTransform);
        }

        void OnEnable()
        {
            UpdateManager.RegisterLateUpdate(this);

            input = IServiceLocator.Default.GetService<InputManager>();
            input.OnZoomPressed += HandleZoom;
            input.OnZoomReleased += HandleZoom;
        }

        void ILateUpdate.LateUpdate()
        {
            rotation.Update(input);
            breathing.Update();
        }

        void OnDisable()
        {
            UpdateManager.UnregisterLateUpdate(this);

            input.OnZoomPressed -= HandleZoom;
            input.OnZoomReleased -= HandleZoom;

            zoom.Dispose();
        }

        void HandleZoom() => zoom.ChangeFOV();

        public void ChangeRunFOV(bool returning) => zoom.ChangeRunFOV(returning);

        public void HandleSway(Vector3 inputVector, float rawXInput)
            => swaying.SwayPlayer(inputVector, rawXInput);
    }
}