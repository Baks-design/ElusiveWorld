using System;
using System.Collections.Generic;
using Assets.Scripts.Internal.Runtime.Core.Systems.Managers.Data;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

namespace Assets.Scripts.Internal.Runtime.Core.App.Input
{
    public class InputEvent
    {
        static readonly List<InputEvent> inputEvents = new();
        readonly KeyControl keyControl;
        float lastKeyPressedTimeStamp = 0f;
        bool pressedLastFrame = false;

        public bool Pressed { get; set; }
        public bool Held { get; set; }
        public bool Released { get; set; }
        public bool DoubleTapped { get; set; }

        public event Action OnPressed = delegate { };
        public event Action OnHeld = delegate { };
        public event Action OnReleased = delegate { };
        public event Action OnDoubleTapped = delegate { };

        public InputEvent(KeyControl keyControl)
        {
            this.keyControl = keyControl;
            inputEvents.Add(this);
        }

        public static void UpdateKeyStates()
        {
            foreach (var inputEvent in inputEvents)
                inputEvent.UpdateKeyState();
        }

        void UpdateKeyState()
        {
            var pressedThisFrame = keyControl.isPressed;

            Pressed = !pressedLastFrame && pressedThisFrame;
            Held = pressedLastFrame && pressedThisFrame;
            Released = pressedLastFrame && !pressedThisFrame;
            DoubleTapped = false;

            if (Pressed)
            {
                DoubleTapped = Time.time - lastKeyPressedTimeStamp < SO_Manager_Input.Instance.DoubleTapThreshold;
                lastKeyPressedTimeStamp = Time.time;
            }

            HandleKeyEventCallbacks();

            pressedLastFrame = pressedThisFrame;
        }

        void HandleKeyEventCallbacks()
        {
            if (Pressed) OnPressed();
            if (Held) OnHeld();
            if (Released) OnReleased();
            if (DoubleTapped) OnDoubleTapped();
        }
    }
}
