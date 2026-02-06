using System;
using System.Collections;
using System.Collections.Generic;
using CozyFarm.Agent;
using CozyFarm.Farming;
using UnityEngine;

namespace CozyFarm
{
    public class FieldSelectionPreview : MonoBehaviour
    {
        [SerializeField] private GameObject _selectionIconPrefab;

        private List<GameObject> _selectionIcons = new();
        private FieldDetector _fieldDetector;

        private void Awake() {
            _fieldDetector = FindObjectOfType<Player>(true).GetComponentInChildren<FieldDetector>();
            if (_fieldDetector == null)
            {
                Debug.LogWarning("Field Detector is null. Tile preview will not work", gameObject);
                return;
            }

            _fieldDetector.OnFieldExited += HideAllIcons;
            _fieldDetector.OnResetDetectedFields += HideAllIcons;
            _fieldDetector.OnPositionDetected += UpdateIcons;
        }

        private void OnDisable() {
            if (_fieldDetector == null) return;
            
            _fieldDetector.OnFieldExited -= HideAllIcons;
            _fieldDetector.OnResetDetectedFields -= HideAllIcons;
            _fieldDetector.OnPositionDetected -= UpdateIcons;
        }

        private void HideAllIcons()
        {
            foreach (GameObject item in _selectionIcons)
            {
                item.SetActive(false);
            }
        }
        private void UpdateIcons(IEnumerable<Vector2> positions)
        {
            HideAllIcons();
            ShowIcons(positions);
        }

        private void ShowIcons(IEnumerable<Vector2> positions)
        {
            int idx = 0;
            foreach (Vector2 pos in positions)
            {
                if (_selectionIcons.Count <= idx)
                {
                    _selectionIcons.Add(Instantiate(_selectionIconPrefab));
                }

                _selectionIcons[idx].SetActive(true);
                _selectionIcons[idx].transform.position = pos;
                _selectionIcons[idx].transform.rotation = Quaternion.identity;
                idx++;
            }
        }
    }
}
