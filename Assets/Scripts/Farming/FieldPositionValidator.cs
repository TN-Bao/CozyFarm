using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CozyFarm.Farming
{
    public class FieldPositionValidator : MonoBehaviour
    {
        [SerializeField] private Tilemap _fieldTilemap;
        [SerializeField] private string _fieldTilemapTag = "Field";

        void Awake()
        {
            if (_fieldTilemap == null)
            {
                _fieldTilemap = FindObjectsOfType<Tilemap>().
                    FirstOrDefault(tilemap => tilemap.CompareTag(_fieldTilemapTag));
            }
            Debug.Assert(_fieldTilemap != null, "Field tilemap reference must be assigned", gameObject);
        }

        public bool IsItFieldTile(Vector2 worldPos)
        {
            return _fieldTilemap.HasTile(_fieldTilemap.WorldToCell(worldPos));
        }

        public List<Vector2> GetValidFieldTiles(List<Vector2> worldPos)
        {
            List<Vector2> validPos = new();
            foreach (Vector2 position in worldPos)
            {
                Vector3Int tilemapPos = _fieldTilemap.WorldToCell(position);
                if (_fieldTilemap.HasTile(tilemapPos))
                {
                    validPos.Add(_fieldTilemap.GetCellCenterWorld(tilemapPos));
                }
            }
            return validPos;
        }

        public Vector2 GetValidFieldTile(Vector2 worldPos)
        {
            if (IsItFieldTile(worldPos) == false)
                throw new System.Exception("Position is invalid for our field Tilemap");
            
            return _fieldTilemap.GetCellCenterWorld(_fieldTilemap.WorldToCell(worldPos));
        }
    }
}
