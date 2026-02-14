using UnityEngine;
using UnityEngine.UI;

namespace CozyFarm.UI
{
    public class ScrollingUI : MonoBehaviour
    {
        [SerializeField] private RectTransform _gridLayoutTransform;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private int _numberOfRows = 0;
        [SerializeField] private InventoryRendererUI _inventoryRenderer;

        private bool _gridReady = false;
        private float _movementStep = 0;

        private void PrepareScrolling()
        {
            DetectoNumerOfRows();
            _movementStep = 1.0f / (_numberOfRows - 2.0f);
            _gridReady = true;  
        }

        private void DetectoNumerOfRows()
        {
            _numberOfRows = _gridLayoutTransform.childCount / _inventoryRenderer.RowSize;
        }

        private Vector2Int GetGridPosCoordinates(int index)
        {
            if (_gridReady == false)
            {
                PrepareScrolling();
            }
            return new Vector2Int(index % _inventoryRenderer.RowSize,
                Mathf.FloorToInt(index / _inventoryRenderer.RowSize));
        }

        public void OnSelectionChanged(int index)
        {
            if (_gridReady == false)
            {
                PrepareScrolling();
            }

            Vector2Int gridPos = GetGridPosCoordinates(index);

            if (gridPos.y < 1)
            {
                _scrollRect.verticalNormalizedPosition = Mathf.Clamp01(1);
            }
            else if (gridPos.y > (_numberOfRows - 1))
            {
                _scrollRect.verticalNormalizedPosition
                    = Mathf.Clamp01(1 - _movementStep*(_numberOfRows - 1)); //0
            } else
            {
                _scrollRect.verticalNormalizedPosition
                    = Mathf.Clamp01(1 - _movementStep*(gridPos.y - 1));
            }
        }
    }
}
