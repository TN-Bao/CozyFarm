using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CozyFarm.DataStorage
{
    public class Inventory : MonoBehaviour
    {
        private InventoryItemData[] _inventoryContent;
        public IEnumerable<InventoryItemData> InventoryContent => _inventoryContent;

        [SerializeField] private int _capacity = 8;
        public int Capacity { get => _capacity; }
        public event Action<IEnumerable<InventoryItemData>> OnUpdateInventory;

        private void Awake() {
            if (_inventoryContent == null)
            {
                _inventoryContent = new InventoryItemData[Capacity];
            }
        }

        public void ChangeCapacity(int newValue)
        {
            if (newValue <= 0) return;

            InventoryItemData[] newInventoryContent = new InventoryItemData[newValue];
            for (int i = 0; i < _capacity; i++)
            {
                if (i >= newValue) break;

                InventoryItemData itemData = _inventoryContent[i];
                if (itemData == null) continue;

                newInventoryContent[i] = new InventoryItemData(itemData.id, itemData.count, itemData.quality, itemData.data);
            }

            _inventoryContent = newInventoryContent;
            _capacity = newValue;
        }

        public int AddItem(InventoryItemData itemData, int stackSize)
        {
            int quantityRemaining = itemData.count;
            if (stackSize > 1)
            {
                for (int i = 0; i < _capacity; i++)
                {
                    if (_inventoryContent[i] != null && itemData.id == _inventoryContent[i].id
                        && _inventoryContent[i].quality == itemData.quality)
                    {
                        int freeSpace = stackSize - _inventoryContent[i].count;
                        int quantityToStore = quantityRemaining;

                        if (quantityRemaining > freeSpace)
                        {
                            quantityToStore = freeSpace;
                            quantityRemaining -= freeSpace;
                        }
                        else
                        {
                            quantityRemaining = 0;
                        }

                        _inventoryContent[i] = new InventoryItemData(itemData.id,
                            _inventoryContent[i].count + quantityToStore, itemData.quality, itemData.data);

                        OnUpdateInventory?.Invoke(InventoryContent);
                        if (quantityRemaining <= 0) return 0;
                    }
                }
            }

            if (quantityRemaining > 0)
            {
                for (int i = 0; i < _capacity; i++)
                {
                    if (_inventoryContent[i] == null)
                    {
                        int quantityToAdd;
                        if (stackSize > 1)
                        {
                            quantityToAdd = quantityRemaining > stackSize ? stackSize : quantityRemaining;
                        }
                        else
                        {
                            quantityToAdd = 1;
                        }

                        _inventoryContent[i] = new InventoryItemData(itemData.id, quantityToAdd, itemData.quality, itemData.data);
                        quantityRemaining -= quantityToAdd;

                        OnUpdateInventory?.Invoke(InventoryContent);
                        if (quantityRemaining <= 0) return 0;
                    }
                }
            }
            return quantityRemaining;
        }

        public bool IsThereSpace(InventoryItemData itemData, int stackSize)
        {
            if (stackSize > 1)
            {
                return _inventoryContent.Any(data => data == null)
                    || _inventoryContent.Where(data => data.id == itemData.id && data.count + itemData.count <= stackSize).Count() > 0;
            }
            return _inventoryContent.Any(data => data == null);
        }

        public override string ToString()
        {
            StringBuilder sb = new();
            sb.Append("Inventory content: ");

            foreach (var item in InventoryContent)
            {
                if (item == null) sb.Append("NULL, ");
                else sb.Append(item.id + $"({item.count}), ");
            }
            return sb.ToString();
        }

        public InventoryItemData GetItemDataAt(int index)
        {
            if (index >= _capacity || index < 0)
            {
                return null;
            }
            return _inventoryContent[index];
        }

        public bool SetItemDataAt(int index, InventoryItemData itemData)
        {
            if (index >= _capacity || index < 0) return false;

            _inventoryContent[index] = itemData;
            OnUpdateInventory?.Invoke(InventoryContent);
            return true;
        }

        public void RemoveAllItem(InventoryItemData item)
        {
            int index = Array.IndexOf(_inventoryContent, item);
            if (index > -1)
            {
                _inventoryContent[index] = null;
                OnUpdateInventory?.Invoke(InventoryContent);
            }
        }

        public bool RemoveAllItemAt(int index)
        {
            if (index >= _capacity || index < 0) return false;

            _inventoryContent[index] = null;
            OnUpdateInventory?.Invoke(InventoryContent);
            return true;
        }

        public bool AddItemAt(int index, InventoryItemData item)
        {
            if (index >= _capacity || index < 0) return false;

            _inventoryContent[index] = item;
            OnUpdateInventory?.Invoke(InventoryContent);
            return true;
        }

        internal void Clear()
        {
            for (int i = 0; i < _inventoryContent.Length; i++)
            {
                _inventoryContent[i] = null;
            }
        }
    }

    public record InventoryItemData(int id, int count, int quality, string data = null);
}

namespace System.Runtime.CompilerServices
{
    public class IsExternalInit
    {
        
    }
}
