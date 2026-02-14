using CozyFarm.Input;
// using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;

namespace CozyFarm.UI
{
    [RequireComponent(typeof(InventoryRendererUI))]
    public class ItemSelectionUI : MonoBehaviour
    {
        [SerializeField] private int _selectedItemIndex = 0;
        private InventoryRendererUI _inventoryRenderer;
        public UnityEvent<Direction, int> OnSelectOutsideOfBounds;
        public UnityEvent<int> OnSelectInsideChange;
        public int SelectedItem => _selectedItemIndex;

        void Awake()
        {
            _inventoryRenderer = GetComponent<InventoryRendererUI>();
        }

        public void EnableController(PlayerInputFarm myInput)
        {
            myInput.OnUIMoveInput += SelectItem;
            if (_inventoryRenderer == null) Awake();

            _inventoryRenderer.SelectItem(_selectedItemIndex);
            SelectItem(Vector2.zero);
        }

        public void DisableController(PlayerInputFarm myInput)
        {
            myInput.OnUIMoveInput -= SelectItem;
        }

        private void SelectItem(Vector2 playerInput)
        {
            Vector2Int input = Vector2Int.RoundToInt(playerInput);

            int newIndex = 0;
            Direction direction = Direction.None;

            (newIndex, direction) = FindDirection(input);

            int CurrentRow = _selectedItemIndex / _inventoryRenderer.RowSize;
            int NewRow = newIndex / _inventoryRenderer.RowSize;
            int CurrentColumn = _selectedItemIndex % _inventoryRenderer.RowSize;
            int NewColumn = newIndex % _inventoryRenderer.RowSize;

            if (newIndex > -1 && newIndex < _inventoryRenderer.InventoryItemCount
                && ((NewRow == CurrentRow) || (NewColumn == CurrentColumn)))
            {
                SelectItemAt(newIndex);
            }
            else
            {
                OnSelectOutsideOfBounds?.Invoke(direction, newIndex);
            }
        }

        private void SelectItemAt(int newIndex)
        {
            _inventoryRenderer.ResetAllSelection(false);
            _selectedItemIndex = newIndex;
            _inventoryRenderer.SelectItem(_selectedItemIndex);
            OnSelectInsideChange?.Invoke(_selectedItemIndex);
        }

        private (int, Direction) FindDirection(Vector2Int input)
        {
            int newIndex = 0;
            Direction direction = Direction.None;

            if (input == Vector2Int.zero)
                return (_selectedItemIndex, Direction.None);
            
            if (input.x == 1)
            {
                newIndex = _selectedItemIndex + 1;
                direction = Direction.Right;
            }
            else if (input.x == -1)
            {
                newIndex = _selectedItemIndex - 1;
                direction = Direction.Left;
            }
            else if (input.y == 1)
            {
                newIndex = _selectedItemIndex - _inventoryRenderer.RowSize;
                direction = Direction.Up;
            }
            else if (input.y == -1)
            {
                newIndex = _selectedItemIndex + _inventoryRenderer.RowSize;
                direction = Direction.Down;
            }
            return (newIndex, direction);
        }

        public void WrapHorizontalMovementSelection(Direction direction, int index)
        {
            if (direction == Direction.Left)
            {
                int wrappedIdx = _selectedItemIndex + _inventoryRenderer.RowSize - 1;
                int currentRow = _selectedItemIndex / _inventoryRenderer.RowSize;
                int newRow = wrappedIdx / _inventoryRenderer.RowSize;

                if (wrappedIdx >= _inventoryRenderer.InventoryItemCount || newRow != currentRow)
                {
                    return;
                }

                _inventoryRenderer.ResetAllSelection(false);
                _selectedItemIndex = wrappedIdx;
                _inventoryRenderer.SelectItem(_selectedItemIndex);
                OnSelectInsideChange?.Invoke(_selectedItemIndex);
            }

            if (direction == Direction.Right)
            {
                int wrappedIdx = _selectedItemIndex - _inventoryRenderer.RowSize + 1;
                int currentRow = _selectedItemIndex / _inventoryRenderer.RowSize;
                int newRow = wrappedIdx / _inventoryRenderer.RowSize;

                if (wrappedIdx < 0 || newRow != currentRow)
                {
                    return;
                }

                _inventoryRenderer.ResetAllSelection(false);
                _selectedItemIndex = wrappedIdx;
                _inventoryRenderer.SelectItem(_selectedItemIndex);
                OnSelectInsideChange?.Invoke(_selectedItemIndex);
            }
        }
    }

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right,
        None
    }
}
