using System;
using System.Collections.Generic;
using CozyFarm.DataStorage;
using CozyFarm.Interaction;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CozyFarm.Farming
{
    public class FieldRenderer : MonoBehaviour
    {
        [SerializeField] private Tilemap _preparedFieldTilemap;
        [SerializeField] private TileBase _preparedFieldTile, _wateredFieldTile;

        Dictionary<Vector3Int, GameObject> _cropVisualRepresentation = new();
        [SerializeField] private GameObject _cropPrefab;

        public Vector3Int GetTilemapTilePosition(Vector3 worldPos)
            => _preparedFieldTilemap.WorldToCell(worldPos);

        public void PrepareFieldAt(Vector3Int fieldCellPos, bool watered = false)
        {
            TileBase tile = watered ? _wateredFieldTile : _preparedFieldTile;
            _preparedFieldTilemap.SetTile(fieldCellPos, tile);
        }

        internal void CreateCropVisualization(Vector3Int tilePos, Sprite cropSprite, bool changeLayerOrder = false)
        {
            _cropVisualRepresentation[tilePos] = Instantiate(_cropPrefab);
            _cropVisualRepresentation[tilePos].transform.position = tilePos
                + new Vector3(0.5f, 0.5f);

            UpdateCropVisualization(tilePos, cropSprite, changeLayerOrder);
        }

        public void WiltCropVisualization(Vector3Int position)
        {
            if (_cropVisualRepresentation[position] != null)
            {
                _cropVisualRepresentation[position].GetComponent<CropRenderer>().WiltCrop();
                if (_cropVisualRepresentation[position]
                    .TryGetComponent(out PickUpInteraction interaction))
                {
                    Destroy(interaction);
                }
            }
            else
            {
                Debug.LogError($"There is no CROP at position {position}", gameObject);
            }
        }

        public void UpdateCropVisualization(Vector3Int tilePos, Sprite cropSprite, bool changeLayerOrder)
        {
            CropRenderer renderer = _cropVisualRepresentation[tilePos].GetComponent<CropRenderer>();

            renderer.SetSprite(cropSprite);
            if (changeLayerOrder)
            {
                renderer.ChangeLayerOrder();
            }
        }

        internal PickUpInteraction MakeCropCollectable(Vector3Int position, CropData cropData,
            int quality, ItemDatabaseSO itemDatabase)
        {
            GameObject cropObj = _cropVisualRepresentation[position];
            ItemData itemData = cropObj.AddComponent<ItemData>();

            itemData.itemDatabaseIndex = cropData.ProducedItemID;
            itemData.itemCount = cropData.ProducedCount;
            itemData.itemQuality = quality;

            PickUpInteraction interaction = cropObj.AddComponent<PickUpInteraction>();

            interaction.ItemDatabase = itemDatabase;
            interaction.UsableTools = cropData.GetCollectTools;
            interaction.OnPickup = new();

            return interaction;
        }

        public void ClearPreparedFields()
        {
            _preparedFieldTilemap.ClearAllTiles();
        }

        public void RemoveCropAt(Vector3Int position)
        {
            if (_cropVisualRepresentation.ContainsKey(position))
                Destroy(_cropVisualRepresentation[position]);

            _cropVisualRepresentation.Remove(position);
        }
    }
}
