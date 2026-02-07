using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace CozyFarm.Input
{
    public class PlayerInputFarm : MonoBehaviour
    {
        [SerializeField] private PlayerInput _input;
        [field : SerializeField] public Vector2 InputValue { get; set; }

        public UnityEvent<Vector2> OnMoveInput;
        public event Action OnPerformAction, OnSwapTool;

        private InputAction _move, _interact, _swapTool;

        void Awake()
        {
            _move = _input.actions["Player/Movement"];
            _interact = _input.actions["Player/Interact"];
            _swapTool = _input.actions["Player/SwapTool"];
        }

        void OnEnable()
        {
            _move.performed += Move;
            _move.canceled += Move;

            _interact.performed += Interact;
            _swapTool.performed += SwapTool;
        }

        void OnDisable()
        {
            _move.performed -= Move;
            _move.canceled -= Move;

            _interact.performed -= Interact;
            _swapTool.performed -= SwapTool;
        }

        private void SwapTool(InputAction.CallbackContext context)
        {
            OnSwapTool?.Invoke();
        }

        public void BlockInput(bool val)
        {
            if (val) _input.enabled = false;
            else _input.enabled = true;
        }

        private void Interact(InputAction.CallbackContext context)
        {
            OnPerformAction?.Invoke();
        }

        private void Move(InputAction.CallbackContext context)
        {
            InputValue = context.ReadValue<Vector2>();
            OnMoveInput?.Invoke(InputValue);
        }
    }
}
