using CozyFarm.Farming;
using CozyFarm.Input;
using CozyFarm.Interaction;
using CozyFarm.Tools;
using UnityEngine;

namespace CozyFarm.Agent
{
    public class Player : MonoBehaviour, IAgent
    {
        [SerializeField] private AgentMover _agentMover;
        [SerializeField] private PlayerInputFarm _playerInput;
        [SerializeField] private AgentAnimation _agentAnim;
        [SerializeField] private InteractionDetector _interactionDetector;
        [SerializeField] private RuntimeAnimatorController _hoeAnimController;
        [SerializeField] private FieldController _fieldController;

        // private Tool _selectedTool = new HandTool(ToolTypes.Hand);
        private Tool _selectedTool = new HoeTool(ToolTypes.Hoe);

        private bool _blocked;
        public bool Blocked
        {
            get { return _blocked; }
            set
            {
                _blocked = value;
                _agentMover.Stopped = _blocked;
                _playerInput.BlockInput(_blocked);
            }
        }

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
        public Tool SelectedTool { get => _selectedTool; }
        public FieldController FieldController => _fieldController;

        private void Start() {
            _selectedTool.Equip(this);
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
        }

        private void PerformAction()
        {
            Debug.Log("Interacting");
            _selectedTool.ToolAnimator = _hoeAnimController;
            _selectedTool.UseTool(this);
        }
    }
}
