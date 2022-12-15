using UnityEngine;

namespace Demonixis.InMoov.Systems.Animations
{
    [RequireComponent(typeof(Animator))]
    public sealed class AnimationManager : MonoBehaviour
    {
        private Animator _animator;
        
        public bool IsTalking;
        public float AnimationSpeed;
        
        private void Start()
        {
            _animator = GetComponent<Animator>();
        }
        
        // FIXME
        // Still a long way to go ;)
        private void LateUpdate()
        {
            _animator.SetBool("IsTalking", IsTalking);
            _animator.speed = AnimationSpeed;
        }
    }
}