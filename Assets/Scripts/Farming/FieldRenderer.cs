using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CozyFarm.Farming
{
    public class FieldRenderer : MonoBehaviour
    {
        [SerializeField] private Tilemap _preparedFieldTilemap;
        [SerializeField] private TileBase _preparedFieldTile;

        Dictionary<Vector3Int, GameObject> _cropVisualRepresentation = new();
        [SerializeField] private GameObject _cropPrefab;

        public Vector3Int GetTilemapTilePosition(Vector3 worldPos)
            => _preparedFieldTilemap.WorldToCell(worldPos);

        public void PrepareFieldAt(Vector3Int fieldCellPos)
        {
            _preparedFieldTilemap.SetTile(fieldCellPos, _preparedFieldTile);
        }

        internal void CreateCropVisualization(Vector3Int tilePos, Sprite cropSprite, bool changeLayerOrder = false)
        {
            _cropVisualRepresentation[tilePos] = Instantiate(_cropPrefab);
            _cropVisualRepresentation[tilePos].transform.position = tilePos
                + new Vector3(0.5f, 0.5f);

            UpdateCropVisualization(tilePos, cropSprite, changeLayerOrder);
        }

        private void UpdateCropVisualization(Vector3Int tilePos, Sprite cropSprite, bool changeLayerOrder)
        {
            CropRenderer renderer = _cropVisualRepresentation[tilePos].GetComponent<CropRenderer>();

            renderer.SetSprite(cropSprite);
            if (changeLayerOrder)
            {
                renderer.ChangeLayerOrder();
            }
        }
    }
}
