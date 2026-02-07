using System;
using System.Collections.Generic;
using CozyFarm.Agent;
using CozyFarm.DataStorage;
using UnityEngine;

namespace CozyFarm.Tools
{
    public class ToolsBag : MonoBehaviour
    {
        [SerializeField] private ItemDatabaseSO _itemDatabase;
        [SerializeField] private List<int> _initialTools;

        private int _selectedIndex = 0;

        private Tool _currentTool;
        public Tool CurrentTool => _currentTool;
        public event Action<int, List<Sprite>, int?> OnToolsBagUpdated;

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
            if (_currentTool != null)
            {
                PutAway(agent);
            }
            _selectedIndex = newIndex;

            if (_selectedIndex >= _initialTools.Count)
            {
                _selectedIndex = 0;
            }
            ItemDescription description = _itemDatabase.GetItemData(_initialTools[_selectedIndex]);
            Debug.Log($"Equipped tool: {description.Name}");

            _currentTool = ToolFactory.CreateTool(description);
            EquipTool(agent);

            SendUpdateMessage();
        }

        private void SendUpdateMessage()
        {
            List<Sprite> sprites = new List<Sprite>();
            foreach (int ID in _initialTools)
            {
                ItemDescription toolDes = _itemDatabase.GetItemData(ID);
                if (toolDes != null)
                {
                    sprites.Add(toolDes.Image);
                }
            }
            OnToolsBagUpdated?.Invoke(_selectedIndex, sprites, null);
        }

        private void EquipTool(IAgent agent)
        {
            _currentTool.Equip(agent);
        }

        private void PutAway(IAgent agent)
        {
            _currentTool.PutAway(agent);
        }
    }
}
