using Demonixis.InMoov.Data;
using Demonixis.InMoov.Servos;
using Demonixis.InMoov.Systems.Animations;
using Demonixis.InMoov.Utils;
using System;
using System.Collections;
using UnityEngine;
using UnityRandom = UnityEngine.Random;

namespace Demonixis.InMoov.Systems
{
    public partial class RandomAnimation : RobotSystem
    {
        private ServoMixerService _servoMixerService;
        private GesturePlayer _gesturePlayer;

        [SerializeField] private ServoAnimation[] _servoActions;
        [SerializeField] private bool _randomHandGestures = true;
        [SerializeField] private Vector2 _handGestureFrequency = new Vector2(1.5f, 6.5f);
        [SerializeField] private bool _randomArmGestures = true;
        [SerializeField] private Vector2 _armGestureFrequency = new Vector2(2.5f, 6.5f);

        public override void Initialize()
        {
            base.Initialize();

            _servoMixerService = Robot.Instance.GetService<ServoMixerService>();
            _gesturePlayer = new GesturePlayer(_servoMixerService);

            foreach (var action in _servoActions)
                StartCoroutine(PlayAnimationAction(action));

           if (_randomHandGestures)
                StartCoroutine(PlayHandGesture());

            if (_randomArmGestures)
                StartCoroutine(PlayArmGesture());
        }

        public override void Dispose()
        {
            base.Dispose();
            StopAllCoroutines();
        }

        private IEnumerator PlayAnimationAction(ServoAnimation servoAnimation)
        {
            while (Started)
            {
                var value = servoAnimation.RandomRange
                    ? (byte)UnityRandom.Range(servoAnimation.Min, servoAnimation.Max)
                    : servoAnimation.NextValue;

                _servoMixerService.SetServoValueInServo(servoAnimation.Servo, value);

                yield return CoroutineFactory.WaitForSeconds(servoAnimation.Frequency);
            }
        }

        private IEnumerator PlayHandGesture()
        {
            var names = Enum.GetNames(typeof(HandGestures));

            while (Started)
            {
                var side = UnityRandom.Range(0, 100) % 3;
                var gestureId = UnityRandom.Range(0, 100) % names.Length;
                var gesture = (HandGestures)gestureId;

                if (side == 2)
                {
                    _gesturePlayer.ApplyHandGesture(true, gesture);
                    _gesturePlayer.ApplyHandGesture(false, gesture);
                }
                else
                {
                    _gesturePlayer.ApplyHandGesture(side == 0, gesture);
                }

                yield return CoroutineFactory.WaitForSeconds(UnityRandom.Range(_handGestureFrequency.x, _handGestureFrequency.y));
            }
        }

        private IEnumerator PlayArmGesture()
        {
            var names = Enum.GetNames(typeof(ArmGestures));

            while (Started)
            {
                var side = UnityRandom.Range(0, 100) % 3;
                var gestureId = UnityRandom.Range(0, 100) % names.Length;
                var gesture = (ArmGestures)gestureId;

                if (side == 2)
                {
                    _gesturePlayer.ApplyArmGesture(true, gesture);
                    _gesturePlayer.ApplyArmGesture(false, gesture);
                }
                else
                {
                    _gesturePlayer.ApplyArmGesture(side == 0, gesture);
                }

                yield return CoroutineFactory.WaitForSeconds(UnityRandom.Range(_armGestureFrequency.x, _armGestureFrequency.y));
            }
        }

#if UNITY_EDITOR

        [ContextMenu("Set Default Values")]
        public void SetupDefaultValues()
        {
            _servoActions = new[]
            {
                ServoAnimation.New(ServoIdentifier.EyeX, 0, 180, 3.0f),
                ServoAnimation.New(ServoIdentifier.EyeY, 0, 180, 3.2f),
                ServoAnimation.New(ServoIdentifier.HeadPitch, 80, 120, 5f),
                ServoAnimation.New(ServoIdentifier.HeadYaw, 80, 120, 4f),
                ServoAnimation.New(ServoIdentifier.PelvisPitchPrimary, 80, 120, 4.5f),
            };
        }

        private void OnValidate()
        {
            if (!Started || !Application.isPlaying) return;
            Dispose();
            Initialize();
        }
#endif
    }
}