using System;
using System.Text;
using CozyFarm.Tools;
using UnityEngine;

namespace CozyFarm.DataStorage
{
    [Serializable]
    public class ItemDescription
    {
        public string Name;
        [Header("General data:"), Space]
        public int ID = -1;
        public Sprite Image;
        public string Description;
        public bool CanBeStacked = false;
        public int StackQuantity = -1;

        [Header("Item data:"), Space]
        public bool CanThrowAway = true;
        public bool Consumable = false;
        public int EnergyBoost;
        public int Price;

        [Header("Tools data:"), Space]
        public ToolTypes ToolType = ToolTypes.None;
        public Vector2Int ToolRange = Vector2Int.zero;
        public RuntimeAnimatorController ToolAnimator;

        [Header("Crop Data:"), Space]
        public int CropTypeIndex = -1;

        [Header("Item Vizualization Data"), Space]
        public GameObject Prefab;

        public string GetDescription()
        {
            StringBuilder stringBuilder = new();
            stringBuilder.Append(Description);
            stringBuilder.Append("\n");

            if (ToolType == ToolTypes.None && CropTypeIndex == -1)
            {
                if (Price > 0)
                    stringBuilder.Append($"Price: {Price} $ \n");

                if (Consumable)
                    stringBuilder.Append($"Energy Boost: {EnergyBoost} \n");
            }

            return stringBuilder.ToString();
        }
    }
}
