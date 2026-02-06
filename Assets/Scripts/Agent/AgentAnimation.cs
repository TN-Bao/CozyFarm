using UnityEngine;
using UnityEngine.Events;

namespace CozyFarm.Agent
{
    [RequireComponent(typeof(Animator))]
    public class AgentAnimation : MonoBehaviour
    {
        private Animator _animator;
        [SerializeField] private string
            directionX = "DirectionX",
            directionY = "DirectionY",
            movingBoolFlag = "Moving",
            pickupFlag = "Pickup",
            swingFlag = "Swing";

        [SerializeField] private ToolAnimation _toolAnim;
    
        [HideInInspector] public UnityEvent OnAnimationEnd;
        public UnityEvent OnFootStep;

        public ToolAnimation ToolAnim { get => _toolAnim; }

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void PlayerActionAnimationEnd()
        {
            OnAnimationEnd?.Invoke();
            OnAnimationEnd.RemoveAllListeners();
        }

        public void PlayFootStep() => OnFootStep?.Invoke();

        public void PlayMovementAnimation(bool val)
            => _animator.SetBool(movingBoolFlag, val);

        public void ChangeDirection(Vector2 direction)
        {
            if (direction.magnitude < 0.1f) return;

            Vector2Int directionInt = Vector2Int.RoundToInt(direction);
            if(directionInt.x != 0)
            {
                directionInt.y = 0;
            }

            _animator.SetFloat(directionX, directionInt.x);
            _animator.SetFloat(directionY, directionInt.y);
        }

        public void PlayAnimation(AnimationType animType)
        {
            if (animType == AnimationType.PickUp)
            {
                _animator.SetTrigger(pickupFlag);
            }

            if (animType == AnimationType.Swing)
            {
                _animator.SetTrigger(swingFlag);
            }
        }
    }

    public enum AnimationType
    {
        None,
        PickUp,
        Swing
    }
}
