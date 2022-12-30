using UnityEngine;

namespace Demonixis.InMoov.Systems
{
    public enum AnimationState
    {
        None = 0,
        Idle,
        Speak,
        Walk,
        Salute,
        Dance
    }

    [RequireComponent(typeof(Animator))]
    public sealed class AnimationManager : MonoBehaviour
    {
        private Animator _animator;
        private AnimationState _animationState = AnimationState.Idle;
        private string[] _animationStateNames;
        private bool[] _animationStates;

        public AnimationState State
        {
            get => _animationState;
            set
            {
                _animationState = value;

                if (_animator == null) return;

                UpdateState();
            }
        }

        public float AnimationSpeed
        {
            get => _animator.speed;
            set => _animator.speed = value;
        }

        private void Start()
        {
            _animator = GetComponent<Animator>();

            _animationStateNames = new[]
            {
                "IsWaiting",
                "IsTalking",
                "IsWalking",
                "IsDancing"
            };

            _animationStates = new bool[_animationStateNames.Length];
            _animationStates[0] = true;
        }

        public void UpdateState()
        {
            ResetStates();

            switch (_animationState)
            {
                case AnimationState.Idle:
                    _animationStates[0] = true;
                    break;
                case AnimationState.Speak:
                    _animationStates[1] = true;
                    break;
                case AnimationState.Salute:
                    _animationStates[0] = true;
                    _animator.SetTrigger("Salute");
                    break;
                case AnimationState.Walk:
                    _animationStates[2] = true;
                    break;
                case AnimationState.Dance:
                    _animationStates[3] = true;
                    break;
            }

            for (var i = 0; i < _animationStateNames.Length; i++)
                _animator.SetBool(_animationStateNames[i], _animationStates[i]);
        }

        private void ResetStates()
        {
            for (var i = 0; i < _animationStates.Length; i++)
            {
                _animationStates[i] = false;
            }
        }
    }
}