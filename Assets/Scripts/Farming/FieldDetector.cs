using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CozyFarm.Farming
{
    public class FieldDetector : MonoBehaviour
    {        
        [SerializeField] private string _fieldTag = "Field";
        [SerializeField] private Transform _interactorCenter;
        [SerializeField] private FieldPositionValidator _fieldPosValidator;
        [SerializeField] private float _checkDelay = 0.01f;

        Coroutine _oldCoroutine = null;
        private Vector2 _interactionDirection;

        public event Action OnFieldExited;
        public event Action OnResetDetectedFields;
        public event Action<IEnumerable<Vector2>> OnPositionDetected;

        private bool _isNearField;
        public bool IsNearField
        {
            get { return _isNearField; }
            private set {
                _isNearField = value;
                if (_isNearField == false)
                {
                    _validSelectionPositions = new();
                    OnFieldExited?.Invoke();
                } }
        }
        
        private List<Vector2> _validSelectionPositions = new();
        public List<Vector2> ValidSelectionPositions
        {
            get { return _validSelectionPositions; }
        }
        

        public Vector2 PositionInFront
            => (Vector2)_interactorCenter.position + _interactionDirection * 0.5f;

        private void Awake()
        {
            _fieldPosValidator = FindObjectOfType<FieldPositionValidator>();
            if (_fieldPosValidator == null)
                Debug.LogWarning("Field position will not be validated without Field Position Validator", gameObject);
        }

        public void StartChecking(Vector2Int detectionRange)
        {
            StopChecking();
            _oldCoroutine = StartCoroutine(CheckField(detectionRange));
        }

        public void StopChecking()
        {
            if (_oldCoroutine != null)
                StopCoroutine(_oldCoroutine);
        }

        private IEnumerator CheckField(Vector2Int detectionRange)
        {
            if (_isNearField && _fieldPosValidator != null && _fieldPosValidator.IsItFieldTile(PositionInFront))
            {
                _validSelectionPositions = DetecValidTiles(detectionRange);
                OnPositionDetected?.Invoke(ValidSelectionPositions);
            }
            else
            {
                _validSelectionPositions.Clear();
                OnResetDetectedFields?.Invoke();
            }

            yield return new WaitForSeconds(_checkDelay);
            _oldCoroutine = StartCoroutine(CheckField(detectionRange));
        }

        public void SetInteractionDirection(Vector2 direction)
        {
            if (direction.magnitude > 0.1f)
            {
                _interactionDirection = direction;
            }
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(_fieldTag))
            {
                IsNearField = true;
            }
        }

        void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag(_fieldTag))
            {
                IsNearField = false;
            }
        }

        void OnDrawGizmosSelected()
        {
            if (Application.isPlaying && _isNearField)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(PositionInFront, 0.2f);

                if (_fieldPosValidator != null && _fieldPosValidator.IsItFieldTile(PositionInFront))
                {
                    Vector2 validPos = _fieldPosValidator.GetValidFieldTile(PositionInFront);
                    Gizmos.DrawWireCube(validPos, Vector2.one);
                }
            }
        }

        //0X0 -> 1 tile in front
        //1x1 -> 3x3 area
        //2x2 -> 5x5 area
        public List<Vector2> DetecValidTiles(Vector2Int detectionRange)
        {
            if (_fieldPosValidator == null)
                return new List<Vector2>();

            int halfX = detectionRange.x;
            int halfY = detectionRange.y;
            int xMax = halfX * 2 + 1;
            int yMax = halfY * 2 + 1;

            List<Vector2> tilesToCheck = new();
            Vector2 positionInFrontCached = PositionInFront;
            for (int x = 0; x < xMax; x++)
            {
                for (int y = 0; y < yMax; y++)
                {
                    tilesToCheck.Add(positionInFrontCached + new Vector2(x - halfX, y - halfY));
                }
            }
            return _fieldPosValidator.GetValidFieldTiles(tilesToCheck);
            // return _fieldPosValidator.GetValidFieldTiles(new List<Vector2>() { PositionInFront });
        }
    }
}
