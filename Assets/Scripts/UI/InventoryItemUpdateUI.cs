using CozyFarm.DataStorage;
using UnityEngine;

namespace CozyFarm.UI
{
    public class InventoryItemUpdateUI : MonoBehaviour
    {
        [SerializeField] private InventoryRendererUI _inventoryRenderer;

        public void UpdateElement(int index, ItemDescription itemDescription,
            InventoryItemData inventoryItem)
        {
            _inventoryRenderer.UpdateItem(index, itemDescription.Image, inventoryItem.count);
        }

        public void ClearElements() => _inventoryRenderer.ResetItems();
    }
}
