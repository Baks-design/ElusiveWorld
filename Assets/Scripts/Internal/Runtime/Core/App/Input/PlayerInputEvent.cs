using Assets.Scripts.Internal.Runtime.Core.Systems.Managers;
using Assets.Scripts.Internal.Runtime.Core.Utilities.Singletons;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Internal.Runtime.Core.App.Input
{
    public class PlayerInputEvent : Singleton<PlayerInputEvent>
    {
        public static InputEvent MeleeInput { get; set; }
        public static InputEvent ThrowInput { get; set; }

        protected override void Awake()
        {
            base.Awake();
            InitInputEvents();
        }

        void Update() => InputEvent.UpdateKeyStates();

        void InitInputEvents()
        {
            var currentKeyboard = Keyboard.current;
            MeleeInput = new InputEvent(currentKeyboard.vKey);
            ThrowInput = new InputEvent(currentKeyboard.fKey);
        }
    }
}
