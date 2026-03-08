using System;
using System.Collections.Generic;
using CozyFarm.DataStorage;
using CozyFarm.Interaction;
using CozyFarm.TimeSystem;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace CozyFarm.Farming
{
    public class FieldController : MonoBehaviour
    {
        private FieldRenderer _fieldRenderer;

        [SerializeField] private FieldData _fieldData;
        [SerializeField] private CropDatabaseSO _cropDatabaseSO;
        [SerializeField] private ItemDatabaseSO _itemDatabase;

        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _preparedFieldSound, _playSeedSound;

        private TimeManager _timeManager;
        private TimeEventArgs _previousTimeData;

        private void Awake()
        {
            _fieldRenderer = FindObjectOfType<FieldRenderer>(true);
            if (_fieldData == null)
            {
                _fieldData = FindObjectOfType<FieldData>();
                if (_fieldData == null)
                    Debug.LogError("Cannot find Field Data", gameObject);
            }

            if (_timeManager = FindObjectOfType<TimeManager>(true))
            {
                _timeManager.OnDayProgress += AffectCrops;
            }
            else
            {
                Debug.LogWarning("Cannot find TimeManager", gameObject);
            }
        }

        private void AffectCrops(object sender, TimeEventArgs timeArgs)
        {
            if (_previousTimeData != null && _previousTimeData.CurrentDay == timeArgs.CurrentDay)
            {
                return;
            }
            _previousTimeData = timeArgs;

            foreach (var keyValue in _fieldData.crops)
            {
                Crop crop = keyValue.Value;
                CropData data = _cropDatabaseSO.GetDataForID(crop.ID);

                if (data == null)
                {
                    throw new Exception($"No data for the crop with ID {crop.ID}");
                }
                if (crop.Dead)
                {
                    continue;
                }
                if (((timeArgs.CurrentSeason + 1) & data.GrowthSeasonIndex) != (timeArgs.CurrentSeason+1))
                {
                    if (timeArgs.SeasonChanged)
                        crop.Dead = true;
                    else continue;
                }
                ModifyCropStatus(crop, data, keyValue.Key);
                if (crop.Regress >= data.WiltThreshold || crop.Dead)
                {
                    crop.Dead = true;
                    WiltCrop(keyValue.Key);
                }
            }
            PrintCropsStatus();
        }

        private void WiltCrop(Vector3Int key)
        {
            if (_fieldRenderer == null) return;
            Vector3Int cropPos = _fieldRenderer.GetTilemapTilePosition(key);
            _fieldRenderer.WiltCropVisualization(cropPos);
        }

        private void ModifyCropStatus(Crop crop, CropData cropData, Vector3Int position)
        {
            if (crop.Ready)
            {
                crop.Regress++;
            }
            else
            {
                //test
                // if (crop.GrowthLevel < 2)
                //     crop.Watered = true;
                crop.Watered = true;
                
                if (crop.Watered)
                {
                    crop.Watered = false;
                    if (crop.Regress > 0)
                    {
                        crop.Regress--;
                    }
                    else
                    {
                        crop.Progress++;
                        if (crop.Progress > cropData.GrowthDelayPerStage)
                        {
                            crop.GrowthLevel++;
                            crop.Progress = 0;

                            UpdateCropAt(position, crop.ID, crop.GrowthLevel);

                            if (crop.GrowthLevel == cropData.Sprites.Count - 1)
                            {
                                crop.Ready = true;
                                ClearFieldAt(position);
                                if (_fieldRenderer != null)
                                {
                                    PickUpInteraction pickUpInteraction = _fieldRenderer
                                        .MakeCropCollectable(position, cropData, crop.GetQuality(), _itemDatabase);
                                    
                                    pickUpInteraction.OnPickup.AddListener(() =>
                                    {
                                        RemoveCropAt(position);
                                    });
                                }
                                return;
                            }
                        }
                    }
                }
                else
                {
                    if (crop.GrowthLevel > 0)
                    {
                        crop.Regress++;
                    }
                }
            }
        }

        private void UpdateCropAt(Vector3Int position, int id, int growthLevel)
        {
            if (_fieldRenderer == null) return;

            Vector3Int tilePos = _fieldRenderer.GetTilemapTilePosition(position);
            CropData data = _cropDatabaseSO.GetDataForID(id);
            if (data == null)
            {
                Debug.LogError($"No data for crop with id {id}", gameObject);
                return;
            }
            else
            {
                _fieldRenderer.UpdateCropVisualization(tilePos, data.Sprites[growthLevel],
                    growthLevel > 0);

                if (growthLevel < 1)
                {
                    _audioSource.PlayOneShot(_playSeedSound);
                }
            }
        }

        private void RemoveCropAt(Vector3Int position)
        {
            _fieldData.crops.Remove(position);
            if (_fieldRenderer != null)
            {
                _fieldRenderer.RemoveCropAt(position);
            }
        }

        private void ClearFieldAt(Vector3Int position)
        {
            _fieldData._preparedFields.Remove(position);
            RecreatePreparedFieldPositions();
        }

        private void RecreatePreparedFieldPositions()
        {
            if (_fieldRenderer == null) return;

            _fieldRenderer.ClearPreparedFields();
            foreach (var fieldPos in _fieldData._preparedFields)
            {
                bool watered = _fieldData.crops.ContainsKey(fieldPos) ?
                    _fieldData.crops[fieldPos].Watered : false;

                _fieldRenderer.PrepareFieldAt(fieldPos, watered);
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

        public bool IsThereCropAt(Vector2 pos)
            => _fieldData.crops.ContainsKey(_fieldRenderer.GetTilemapTilePosition(pos));

        public void WaterCropAt(Vector2 pos)
        {
            Vector3Int titlePos = _fieldRenderer.GetTilemapTilePosition(pos);
            bool result = WaterCropUpdateAt(titlePos);

            if (result == false) return;

            _fieldRenderer.WaterCropAt(titlePos);
        }

        private bool WaterCropUpdateAt(Vector3Int titlePos)
        {
            if (_fieldData.crops.ContainsKey(titlePos) == false) return false;
            _fieldData.crops[titlePos].Watered = true;
            return true;
        }
    }
}
