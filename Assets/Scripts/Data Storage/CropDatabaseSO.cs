using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CozyFarm.DataStorage
{
    [CreateAssetMenu]
    public class CropDatabaseSO : ScriptableObject
    {
        [SerializeField] private List<CropData> _cropData = new();
        
        public CropData GetDataForID(int cropTypeIndex)
        {
            return _cropData.Where(crop => crop.ID == cropTypeIndex).FirstOrDefault();
        }
    }
}
