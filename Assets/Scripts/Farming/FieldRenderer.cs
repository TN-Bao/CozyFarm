using UnityEngine;
using UnityEngine.Tilemaps;

namespace CozyFarm
{
    public class FieldRenderer : MonoBehaviour
    {
        [SerializeField] private Tilemap _preparedFieldTilemap;
        [SerializeField] private TileBase _preparedFieldTile;

        public Vector3Int GetTilemapTilePosition(Vector3 worldPos)
            => _preparedFieldTilemap.WorldToCell(worldPos);

        public void PrepareFieldAt(Vector3Int fieldCellPos)
        {
            _preparedFieldTilemap.SetTile(fieldCellPos, _preparedFieldTile);
        }
    }
}
