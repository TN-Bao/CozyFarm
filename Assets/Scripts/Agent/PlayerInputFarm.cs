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
        public event Action OnPerformAction, OnSwapTool, OnToggleInventory;
        public event Action OnUIExit, OnUIToggleInventory, OnUIInteract;
        public event Action<Vector2> OnUIMoveInput;

        private InputAction _move, _interact, _swapTool, _toggleInventory;
        private InputAction _moveUI, _interactUI, _toggleInventoryUI, _exitUI;

        void Awake()
        {
            _move = _input.actions["Player/Movement"];
            _interact = _input.actions["Player/Interact"];
            _swapTool = _input.actions["Player/SwapTool"];
            _toggleInventory = _input.actions["Player/ToggleInventory"];

            _moveUI = _input.actions["UI/Movement"];
            _interactUI = _input.actions["UI/Interact"];
            _toggleInventoryUI = _input.actions["UI/ToggleInventory"];
            _exitUI = _input.actions["UI/Exit"];
        }

        void OnEnable()
        {
            _move.performed += Move;
            _move.canceled += Move;
            _interact.performed += Interact;
            _swapTool.performed += SwapTool;
            _toggleInventory.performed += ToggleInventory;

            _moveUI.performed += MoveUI;
            _moveUI.canceled += MoveUI;
            _interactUI.performed += InteractUI;
            _toggleInventoryUI.performed += ToggleInventoryUI;
            _exitUI.performed += ExitUI;
        }

        void OnDisable()
        {
            _move.performed -= Move;
            _move.canceled -= Move;
            _interact.performed -= Interact;
            _swapTool.performed -= SwapTool;
            _toggleInventory.performed -= ToggleInventory;

            _moveUI.performed -= MoveUI;
            _moveUI.canceled -= MoveUI;
            _interactUI.performed -= InteractUI;
            _toggleInventoryUI.performed -= ToggleInventoryUI;
            _exitUI.performed -= ExitUI;
        }

        public void EnableUIActionMap()
        {
            _input.SwitchCurrentActionMap("UI");
        }

        public void EnableDefaultActionMap()
        {
            _input.SwitchCurrentActionMap("Player");
        }

        private void ExitUI(InputAction.CallbackContext context)
        {
            OnUIExit?.Invoke();
        }

        private void ToggleInventoryUI(InputAction.CallbackContext context)
        {
            OnUIToggleInventory?.Invoke();
        }

        private void InteractUI(InputAction.CallbackContext context)
        {
            OnUIInteract?.Invoke();
        }

        private void MoveUI(InputAction.CallbackContext context)
        {
            Vector2 input = context.ReadValue<Vector2>();
            OnUIMoveInput?.Invoke(input);
        }

        private void ToggleInventory(InputAction.CallbackContext context)
        {
            OnToggleInventory?.Invoke();
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
