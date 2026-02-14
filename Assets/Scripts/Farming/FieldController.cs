using System;
using System.Collections.Generic;
using CozyFarm.DataStorage;
using UnityEngine;

namespace CozyFarm.Farming
{
    public class FieldController : MonoBehaviour
    {
        private FieldRenderer _fieldRenderer;

        [SerializeField] private FieldData _fieldData;
        [SerializeField] private CropDatabaseSO _cropDatabaseSO;

        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _preparedFieldSound, _playSeedSound;

        private void Awake()
        {
            _fieldRenderer = FindObjectOfType<FieldRenderer>(true);
            if (_fieldData == null)
            {
                _fieldData = FindObjectOfType<FieldData>();
                if (_fieldData == null)
                    Debug.LogError("Cannot find Field Data", gameObject);
            }
        }

        public void PrepareFieldAt(Vector2 worldPos)
        {
            if (_fieldRenderer == null) return;

            Vector3Int titlePos = _fieldRenderer.GetTilemapTilePosition(worldPos);
            if (_fieldData._preparedFields.Contains(titlePos))
            {
                return;
            }

            _fieldRenderer.PrepareFieldAt(titlePos);
            _fieldData._preparedFields.Add(titlePos);
            _audioSource.PlayOneShot(_preparedFieldSound);
        }

        public bool CanIPlaceCropsHere(Vector2 pos)
        {
            Vector3Int titlePos = _fieldRenderer.GetTilemapTilePosition(pos);
            return _fieldData._preparedFields.Contains(titlePos)
                && _fieldData.crops.ContainsKey(titlePos) == false;
        }

        public void PlaceCropAt(Vector2 pos, int cropID, int growthLevel = 0, bool playSound = true)
        {
            if (_fieldRenderer == null) return;

            Vector3Int titlePos = _fieldRenderer.GetTilemapTilePosition(pos);
            if(_fieldData.crops.ContainsKey(titlePos) == false)
            {
                _fieldData.crops[titlePos] = new Crop(cropID);
            }

            CropData data = _cropDatabaseSO.GetDataForID(cropID);
            if (data == null)
            {
                Debug.LogError($"No data found for id {cropID}");
                return;
            }
            Debug.Log("Creating visualization for the crop");
            _fieldRenderer.CreateCropVisualization(titlePos, data.Sprites[growthLevel],
                growthLevel > 0);

            if (playSound)
            {
                _audioSource.PlayOneShot(_playSeedSound);
            }
            PrintCropsStatus();
        }

        public void PrintCropsStatus()
        {
            _fieldData.PrintCropStatus();
        }
    }
}
