using System;
using System.Collections.Generic;
using CozyFarm.Tools;
using UnityEngine;

namespace CozyFarm.DataStorage
{
    [Serializable]
    public class CropData: ISerializationCallbackReceiver
    {
        public string Name;
        [Min(0)] public int ID;
        [Min(0)] public int ProducedItemID;
        public List<Sprite> Sprites;
        [Min(0)] public int GrowthDelayPerStage;
        [Min(1)] public int WiltThreshold;

        [SerializeField] private Seasons _growthSeason;
        [field: SerializeField] public int GrowthSeasonIndex { get; private set; }

        [field: SerializeField] public int ProducedCount { get; private set; }
        [SerializeField] private List<ToolTypes> _collectionTools;
        public List<ToolTypes> GetCollectTools => new List<ToolTypes>(_collectionTools);

        public void OnBeforeSerialize()
        {
            return;
        }

        public void OnAfterDeserialize()
        {
            GrowthSeasonIndex = (int)_growthSeason;
        }
    }
}

namespace CozyFarm
{
    [Flags]
    public enum Seasons
    {
        Spring = 1,
        Summer = 2,
        Autumn = 4,
        Winter = 8
    }
}
