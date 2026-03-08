using System;
using System.Collections.Generic;
using System.Linq;
using CozyFarm.Agent;
using CozyFarm.DataStorage;
using Unity.VisualScripting;
using UnityEngine;

namespace CozyFarm.Tools
{
    public class ToolsBag : MonoBehaviour
    {
        [SerializeField] private ItemDatabaseSO _itemDatabase;
        [SerializeField] private List<int> _initialTools;
        [SerializeField] private Inventory _toolsBagInventory;
        [SerializeField] private int _handToolID = 4;

        private int _selectedIndex = 0;

        private List<Tool> _newBag;
        public Tool CurrentTool => _newBag[_selectedIndex];
        public event Action<int, List<Sprite>, int?> OnToolsBagUpdated;

        private void Start() {
            for (int i = 0; i < _initialTools.Count; i++)
            {
                ItemDescription description = _itemDatabase.GetItemData(_initialTools[i]);
                string data = null;
                int quantity = 1;
                if (description.ToolType == ToolTypes.SeedPlacer)
                {
                    data = JsonUtility.ToJson(new SeeToolData
                    {
                        cropID = description.CropTypeIndex,
                        quantity = 2
                    });
                    quantity = 2;
                }
                _toolsBagInventory.AddItem(new InventoryItemData(description.ID, quantity, -1, data),
                    description.StackQuantity);
            }

            UpdateToolsBag(_toolsBagInventory.InventoryContent);
        }

        private void UpdateToolsBag(IEnumerable<InventoryItemData> inventoryContent)
        {
            _newBag = new();
            AddDefaultHandTool();
            int index = 0;

            foreach (InventoryItemData tool in inventoryContent)
            {
                if (tool != null)
                {
                    ItemDescription toolDescription = _itemDatabase.GetItemData(tool.id);
                    if (toolDescription == null || toolDescription.ToolType == ToolTypes.None)
                    {
                        Debug.LogError($"Loaded Tool with index {tool.id} is not present in database or None");
                    }
                    Tool newTool = ToolFactory.CreateTool(toolDescription, tool.data);
                    if (newTool is IQuantity)
                    {
                        ((IQuantity)newTool).Quantity = tool.count;
                        _toolsBagInventory.AddItemAt(index, new InventoryItemData(toolDescription.ID, tool.count, tool.quality,
                            newTool.GetDataToSave()));
                    }
                    _newBag.Add(newTool);
                }
                index++;
            }

            if (_selectedIndex >= _newBag.Count)
                _selectedIndex = 0;
        }

        private void AddDefaultHandTool()
        {
            ItemDescription handToolDescription = _itemDatabase.GetItemData(_handToolID);
            Tool handTool = ToolFactory.CreateTool(handToolDescription, null);

            _newBag.Add(handTool); //handTool -> 0
        }

        public void Initialize(IAgent agent)
        {
            SwapTool(_selectedIndex, agent);
        }

        public void SelectedNextTool(IAgent agent)
        {
            SwapTool(_selectedIndex + 1, agent);
        }

        private void SwapTool(int newIndex, IAgent agent)
        {
            if (_newBag[_selectedIndex] != null)
            {
                PutAway(agent);
            }

            _selectedIndex = newIndex;
            if (_selectedIndex >= _newBag.Count)
            {
                _selectedIndex = 0;
            }
            ItemDescription description = _itemDatabase.GetItemData(_newBag[_selectedIndex].ItemIndex);
            Debug.Log($"Equipped tool: {description.Name}");

            // _newBag[_selectedIndex] = ToolFactory.CreateTool(description);
            EquipTool(agent);
            SendUpdateMessage();
        }

        private void SendUpdateMessage()
        {
            int? count = null;
            ItemDescription selectedToolDescription = _itemDatabase.GetItemData(
                _newBag[_selectedIndex].ItemIndex);
            if (selectedToolDescription.ToolType == ToolTypes.SeedPlacer)
            {
                count = _toolsBagInventory.GetItemDataAt(_selectedIndex - 1).count;
            }
            else if (selectedToolDescription.ToolType == ToolTypes.WateringCan)
            {
                count = ((WateringTool)CurrentTool).NumberOfUses;
            }

            List<Sprite> sprites = new List<Sprite>();
            foreach (Tool tool in _newBag)
            {
                ItemDescription toolDes = _itemDatabase.GetItemData(tool.ItemIndex);
                if (toolDes != null)
                {
                    sprites.Add(toolDes.Image);
                }
            }
            OnToolsBagUpdated?.Invoke(_selectedIndex, sprites, count);
        }

        private void EquipTool(IAgent agent)
        {
            _newBag[_selectedIndex].Equip(agent);
            _newBag[_selectedIndex].OnFinishedAction += UpdateInventoryData;
        }

        private void UpdateInventoryData(IAgent agent)
        {
            Tool tool = _newBag[_selectedIndex];
            string data = tool.GetDataToSave();
            if (string.IsNullOrEmpty(data)) return;

            // hand is NOT in the inventory
            int inventoryIndex = _selectedIndex - 1;
            if(inventoryIndex >= 0)
            {
                if (tool.IsToolStillValid())
                {
                    // modified the item
                    int quantity = 1;
                    if (tool is IQuantity)
                    {
                        quantity = ((IQuantity)tool).Quantity;
                    }

                    _toolsBagInventory.AddItemAt(inventoryIndex,
                        new InventoryItemData(tool.ItemIndex, quantity, -1, data));
                } else
                {
                    // recreate the inventory
                    List<InventoryItemData> items = _toolsBagInventory.InventoryContent.ToList();
                    _toolsBagInventory.Clear();

                    for (int i = 0; i < items.Count; i++)
                    {
                        if (i == inventoryIndex || items[i] == null)
                        {
                            continue;
                        }

                        _toolsBagInventory.AddItem(items[i], _itemDatabase.GetItemData(items[i].id).StackQuantity);
                    }
                    UpdateToolsBag(_toolsBagInventory.InventoryContent);
                }
                SwapTool(_selectedIndex, agent);
            }
        }

        private void PutAway(IAgent agent)
        {
            _newBag[_selectedIndex].PutAway(agent);
            _newBag[_selectedIndex].OnFinishedAction = null;
            _newBag[_selectedIndex].OnPerformAction = null;
            _newBag[_selectedIndex].OnStartedAction = null;
        }

        public void RestoreCurrentTool(IAgent agent)
        {
            if (CurrentTool.ToolType == ToolTypes.WateringCan)
            {
                ((WateringTool)CurrentTool).Refill();
            }

            UpdateInventoryData(agent);
        }
    }
}
