using System.Collections.Generic;
using CozyFarm.Agent;
using CozyFarm.DataStorage;
using CozyFarm.Tools;
using UnityEngine;
using UnityEngine.Events;

namespace CozyFarm.Interaction
{
    [RequireComponent(typeof(ItemData))]
    public class PickUpInteraction : MonoBehaviour, IInteractable
    {
        [field: SerializeField]
        public List<ToolTypes> UsableTools { get; set; }
            = new List<ToolTypes>();
        
        [SerializeField] private bool _destroyAfterPickup = true;
        [field: SerializeField] public ItemDatabaseSO ItemDatabase { get; set; }
        public UnityEvent OnPickup;

        private ItemData _itemData;

        private void Awake() {
            _itemData = GetComponent<ItemData>();
        }

        public bool CanInteract(IAgent agent)
            => UsableTools.Contains(agent.ToolsBag.CurrentTool.ToolType);

        public void Interact(IAgent agent)
        {
            // agent.Inventory.AddItem(new InventoryItemData(0, 1, -1), 1);

            InventoryItemData item = new InventoryItemData(_itemData.itemDatabaseIndex,
                _itemData.itemCount, _itemData.itemQuality);
            ItemDescription description = ItemDatabase.GetItemData(_itemData.itemDatabaseIndex);
            int stackSize = description.CanBeStacked ? description.StackQuantity : -1;

            if (agent.Inventory != null && _itemData.itemCount > 0
                && agent.Inventory.IsThereSpace(item, stackSize))
            {
                agent.Inventory.AddItem(item, stackSize);
                Debug.Log(agent.Inventory);
                OnPickup?.Invoke();

                if (_destroyAfterPickup)
                {
                    Debug.Log($"Destroying {description.Name} {gameObject.name}");
                    Destroy(gameObject);
                }
                else
                {
                    
                    _itemData.itemCount = 0;
                }
            }
        }
    }
}
