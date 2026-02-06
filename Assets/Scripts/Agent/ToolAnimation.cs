using UnityEngine;

namespace CozyFarm
{
    [RequireComponent(typeof(Animator))]
    public class ToolAnimation : MonoBehaviour
    {
        private Animator _animator;
        [SerializeField] private string
            directionX = "DirectionX",
            directionY = "DirectionY",
            useFlag = "Use";

        private void Awake()
        {
            _animator = GetComponent<Animator>();    
        }

        public void SetAnimatorController(RuntimeAnimatorController animController)
        {
            _animator.runtimeAnimatorController = animController;
        }

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

        public void PlayAnimation()
        {
            _animator.SetTrigger(useFlag);
        }
    }
}
