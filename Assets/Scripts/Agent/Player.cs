using System;
using CozyFarm.DataStorage;
using CozyFarm.Farming;
using CozyFarm.Input;
using CozyFarm.Interaction;
using CozyFarm.Tools;
using CozyFarm.UI;
using UnityEngine;
using UnityEngine.Events;

namespace CozyFarm.Agent
{
    public class Player : MonoBehaviour, IAgent
    {
        [SerializeField] private AgentMover _agentMover;
        [SerializeField] private PlayerInputFarm _playerInput;
        [SerializeField] private AgentAnimation _agentAnim;
        [SerializeField] private InteractionDetector _interactionDetector;
        [SerializeField] private ItemDatabaseSO _itemDatabase;
        [SerializeField] private FieldController _fieldController;
        [SerializeField] private ToolSelectionUI _toolSelectionUI;
        [field: SerializeField] public ToolsBag ToolsBag { get; private set; }


        private bool _blocked;
        public bool Blocked
        {
            get { return _blocked; }
            set
            {
                _blocked = value;
                _agentMover.Stopped = _blocked;
                if (_blocked)
                    _agentAnim.PlayMovementAnimation(false);
                _playerInput.BlockInput(_blocked);
            }
        }

        [SerializeField]
        private Inventory _inventory;
        public Inventory Inventory => _inventory;
        

        [SerializeField]
        private FieldDetector _fieldDetector;
        public FieldDetector FieldDetectorObject
        {
            get { return _fieldDetector; }
        }

        public AgentMover AgentMover { get => _agentMover; }
        public PlayerInputFarm PlayerInput { get => _playerInput; }
        public AgentAnimation AgentAnim { get => _agentAnim; }
        public InteractionDetector InteractionDetector { get => _interactionDetector; }
        public FieldController FieldController => _fieldController;
        public UnityEvent<Inventory> OnToggleInventory;


        private void Start() {
            ToolsBag.Initialize(this);
        }

        void OnEnable()
        {
            _playerInput.OnMoveInput.AddListener(_agentMover.SetMovementInput);
            _playerInput.OnMoveInput.AddListener(_agentAnim.ChangeDirection);
            _playerInput.OnMoveInput.AddListener(_agentAnim.ToolAnim.ChangeDirection);

            _playerInput.OnMoveInput.AddListener(_interactionDetector.SetInteractionDirection);
            _playerInput.OnMoveInput.AddListener(_fieldDetector.SetInteractionDirection);

            _agentMover.OnMove += _agentAnim.PlayMovementAnimation;
            _playerInput.OnPerformAction += PerformAction;
            _playerInput.OnSwapTool += SwapTool;
            _playerInput.OnToggleInventory += ToggleInventory;

            ToolsBag.OnToolsBagUpdated += _toolSelectionUI.UpdateUI;
        }

        void OnDisable()
        {
            _playerInput.OnMoveInput.RemoveListener(_agentMover.SetMovementInput);
            _playerInput.OnMoveInput.RemoveListener(_agentAnim.ChangeDirection);
            _playerInput.OnMoveInput.RemoveListener(_agentAnim.ToolAnim.ChangeDirection);

            _playerInput.OnMoveInput.RemoveListener(_interactionDetector.SetInteractionDirection);
            _playerInput.OnMoveInput.RemoveListener(_fieldDetector.SetInteractionDirection);

            _agentMover.OnMove -= _agentAnim.PlayMovementAnimation;
            _playerInput.OnPerformAction -= PerformAction;
            _playerInput.OnSwapTool -= SwapTool;
            _playerInput.OnToggleInventory -= ToggleInventory;

            ToolsBag.OnToolsBagUpdated -= _toolSelectionUI.UpdateUI;
        }

        private void ToggleInventory()
        {
            OnToggleInventory?.Invoke(Inventory);
        }

        private void SwapTool()
        {
            ToolsBag.SelectedNextTool(this);
        }

        private void PerformAction()
        {
            Debug.Log("Interacting");
            ToolsBag.CurrentTool.UseTool(this);
        }
    }
}
