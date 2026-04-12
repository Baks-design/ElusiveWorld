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
        readonly CharactersLookFlags flags = new();
        CameraRotation rotation;
        CameraSwaying swaying;
        CameraZoom zoom;
        CameraBreathing breathing;
        InputManager input;

        void Awake()
        {
            input = IServiceLocator.Default.GetService<InputManager>();
            rotation = new(settings, yawTransform, pitchTransform, input);
            swaying = new(settings, camTransform);
            zoom = new(this, settings, cam);
            breathing = new(flags, settings, camTransform);
        }

        void OnEnable()
        {
            UpdateManager.RegisterLateUpdate(this);
            InputSubscribe();
        }

        void ILateUpdate.LateUpdate()
        {
            rotation.Update();
            breathing.Update();
        }

        void OnDisable()
        {
            UpdateManager.UnregisterLateUpdate(this);
            InputUnsubscribe();
        }

        void InputSubscribe()
        {
            if (input == null) return;
            input.OnZoomPressed += zoom.ChangeFOV;
            input.OnZoomReleased += zoom.ChangeFOV;
        }

        void InputUnsubscribe()
        {
            if (input == null) return;
            input.OnZoomPressed -= zoom.ChangeFOV;
            input.OnZoomReleased -= zoom.ChangeFOV;
        }

        public void ChangeRunFOV(bool returning) =>
            zoom.ChangeRunFOV(returning);

        public void HandleSway(Vector3 inputVector, float rawXInput) =>
            swaying.SwayPlayer(inputVector, rawXInput);
    }
}