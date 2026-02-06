using System;
using UnityEngine;

namespace CozyFarm.Agent
{
    public class AgentMover : MonoBehaviour
    {
        public Vector2 MovementInputValue { get; set; }
        public Action<bool> OnMove;

        [SerializeField] private Rigidbody2D _rb;
        [SerializeField] private float _moveSpeed = 2f;
        [SerializeField] private CollisionDetector _collisionDetector;

        private bool _stopped;
        public bool Stopped
        {
            get { return _stopped; }
            set { _stopped = value; }
        }
        
        void FixedUpdate()
        {
            if (_stopped) return;
            
            Vector2 velocity = MovementInputValue * _moveSpeed;

            float disToMoveThisFrame = velocity.magnitude * Time.deltaTime;
            if (_collisionDetector.IsMovementValid(MovementInputValue, disToMoveThisFrame) == false)
            {
                velocity = Vector2.zero;
            }

            OnMove?.Invoke(velocity.magnitude > 0.1f);
            _rb.MovePosition(_rb.position + velocity * Time.fixedDeltaTime);
        }

        internal void SetMovementInput(Vector2 value)
        {
            MovementInputValue = value;
        }
    }
}
