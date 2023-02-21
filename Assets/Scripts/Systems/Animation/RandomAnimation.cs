using Demonixis.InMoov.Servos;
using Demonixis.InMoov.Utils;
using System;
using System.Collections;
using UnityEngine;
using UnityRandom = UnityEngine.Random;

namespace Demonixis.InMoov.Systems
{
    public class RandomAnimation : RobotSystem
    {
        [Serializable]
        public class ServoAnimation
        {
            private int _index;

            public ServoIdentifier Servo;
            public bool RandomRange;
            public byte Min;
            public byte Max;
            public byte[] Sequence;
            public float Frequency;

            public int Cursor
            {
                set
                {
                    _index = value;

                    if (_index < 0)
                        _index = Sequence.Length - 1;
                    else if (_index > Sequence.Length)
                        _index = 0;
                }
                get => _index;
            }

            public static ServoAnimation New(ServoIdentifier id, byte min, byte max, float freq)
            {
                return new ServoAnimation
                {
                    Servo = id,
                    RandomRange = true,
                    Min = min,
                    Max = max,
                    Frequency = freq
                };
            }

            public byte NextValue => (byte)(Sequence != null ? Sequence[Cursor] : 0);
        }

        private ServoMixerService _servoMixerService;

        [SerializeField] private ServoAnimation[] _servoActions;

        public override void Initialize()
        {
            base.Initialize();

            _servoMixerService = Robot.Instance.GetService<ServoMixerService>();

            foreach (var action in _servoActions)
                StartCoroutine(PlayAnimationAction(action));
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
                ServoAnimation.New(ServoIdentifier.LeftShoulderYaw, 80, 140, 5f),
                ServoAnimation.New(ServoIdentifier.LeftShoulderPitch, 80, 140, 6f),
                ServoAnimation.New(ServoIdentifier.LeftShoulderRoll, 80, 140, 5f),
                ServoAnimation.New(ServoIdentifier.LeftElbowPitch, 50, 160, 3.5f),
                ServoAnimation.New(ServoIdentifier.LeftWristRoll, 0, 180, 4f),
                ServoAnimation.New(ServoIdentifier.RightShoulderYaw, 80, 140, 5f),
                ServoAnimation.New(ServoIdentifier.RightShoulderPitch, 80, 140, 6f),
                ServoAnimation.New(ServoIdentifier.RightShoulderRoll, 80, 140, 5f),
                ServoAnimation.New(ServoIdentifier.RightElbowPitch, 50, 160, 3.5f),
                ServoAnimation.New(ServoIdentifier.LeftWristRoll, 0, 180, 4f),
                ServoAnimation.New(ServoIdentifier.PelvisPitchPrimary, 80, 120, 4.5f),
                ServoAnimation.New(ServoIdentifier.LeftFingerThumb, 0, 180, 4),
                ServoAnimation.New(ServoIdentifier.LeftFingerIndex, 0, 180, 4),
                ServoAnimation.New(ServoIdentifier.LeftFingerMiddle, 0, 180, 4),
                ServoAnimation.New(ServoIdentifier.LeftFingerRing, 0, 180, 4),
                ServoAnimation.New(ServoIdentifier.LeftFingerPinky, 0, 180, 4),
                ServoAnimation.New(ServoIdentifier.RightFingerThumb, 0, 180, 4),
                ServoAnimation.New(ServoIdentifier.RightFingerIndex, 0, 180, 4),
                ServoAnimation.New(ServoIdentifier.RightFingerMiddle, 0, 180, 4),
                ServoAnimation.New(ServoIdentifier.RightFingerRing, 0, 180, 4),
                ServoAnimation.New(ServoIdentifier.RightFingerPinky, 0, 180, 4),
            };

            foreach (var t in _servoActions)
                t.Frequency = 2.5f;
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