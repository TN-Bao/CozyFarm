using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CozyFarm.Farming
{
    public class FieldController : MonoBehaviour
    {
        private FieldRenderer _fieldRenerer;
        [SerializeField] private List<Vector3Int> _preparedFields = new();
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _preparedFieldSound;

        private void Awake()
        {
            _fieldRenerer = FindObjectOfType<FieldRenderer>(true);    
        }

        public void PrepareFieldAt(Vector2 worldPos)
        {
            if (_fieldRenerer == null) return;

            Vector3Int titlePos = _fieldRenerer.GetTilemapTilePosition(worldPos);
            if (_preparedFields.Contains(titlePos))
            {
                return;
            }

            _fieldRenerer.PrepareFieldAt(titlePos);
            _preparedFields.Add(titlePos);
            _audioSource.PlayOneShot(_preparedFieldSound);
        }
    }
}
