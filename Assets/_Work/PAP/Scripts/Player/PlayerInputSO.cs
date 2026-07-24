using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Work.PAP.Scripts
{
    [CreateAssetMenu(fileName = "Player Input", menuName = "SO/Player Input SO", order = 0)]
    public class PlayerInputSO : ScriptableObject, Controls.IPlayerActions
    {
        private Controls _controls;

        public Vector2 MovementInput { get; private set; }
        public event Action OnDashKeyPressed;
        public event Action OnMouseKeyPressed;
        private void OnEnable()
        {
            if (_controls == null)
            {
                _controls = new Controls();
                _controls.Player.SetCallbacks(this);
            }
            _controls.Enable();
        }

        private void OnDisable()
        {
            if (_controls != null)
            {
                _controls.Player.Disable();
            }
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            MovementInput = context.ReadValue<Vector2>();
        }

        public void OnDash(InputAction.CallbackContext context)
        {
            if (context.performed) OnDashKeyPressed?.Invoke();
        }

        public void OnMouse(InputAction.CallbackContext context)
        {
            if (context.performed) OnMouseKeyPressed?.Invoke();
        }
    }
}