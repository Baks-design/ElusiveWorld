using UnityEngine;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Services;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Updates.Variable;
using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Updates.Variable.Interfaces;
using ElusiveWorld.Core.Assets.Scripts.Systems.Input;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

namespace ElusiveWorld.Core.Assets.Scripts.Behaviours.Characters
{
    public class CharactersLook : MonoBehaviour, ILateUpdate
    {
        [SerializeField] CinemachineCamera cam;
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
            zoom = new(settings, cam);
            breathing = new(settings, camTransform);
        }

        void OnEnable()
        {
            UpdateManager.RegisterLateUpdate(this);

            input = IServiceLocator.Default.GetService<InputManager>();
            if (input != null)
            {
                input.OnZoomPressed += OnZoomPressed;
                input.OnZoomReleased += OnZoomReleased;
            }
        }

        void ILateUpdate.LateUpdate()
        {
            rotation.Update(input);
            breathing.Update();
        }

        void OnDisable()
        {
            UpdateManager.UnregisterLateUpdate(this);

            if (input != null)
            {
                input.OnZoomPressed -= OnZoomPressed;
                input.OnZoomReleased -= OnZoomReleased;
            }
        }

        void OnZoomPressed() => zoom.ChangeFOV(this);

        void OnZoomReleased() => zoom.ChangeFOV(this);

        public void ChangeRunFOV(bool returning) 
            => zoom.ChangeRunFOV(this, returning);

        public void HandleSway(Vector3 inputVector, float rawXInput)
            => swaying.SwayPlayer(inputVector, rawXInput);
    }
}