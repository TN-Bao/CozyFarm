using UnityEngine;

namespace CozyFarm.Agent
{
    public class CollisionDetector : MonoBehaviour
    {
        [SerializeField] private ContactFilter2D _contactFilter;
        [SerializeField] private Collider2D _movementCollider;
        [SerializeField] private float _safetyOffset = 0.01f;
        [SerializeField] private int _collisionResult = 0;

        private RaycastHit2D[] _hitObjects = new RaycastHit2D[8];

        void Awake()
        {
            Debug.Assert(_movementCollider != null, "Collider cannot be null", gameObject);
        }

        public bool IsMovementValid(Vector2 movementDir, float disToMoveThisFrame)
        {
            _collisionResult = _movementCollider.Cast(
                movementDir, _contactFilter, _hitObjects,
                disToMoveThisFrame + _safetyOffset);

            Debug.DrawRay(transform.position + (Vector3)_movementCollider.offset,
                movementDir*(disToMoveThisFrame + _safetyOffset),
                _collisionResult == 0 ? Color.green : Color.red);

            return _collisionResult == 0;
        }
    }
}
