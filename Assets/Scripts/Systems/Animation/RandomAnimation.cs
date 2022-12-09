﻿using Demonixis.InMoov.Servos;
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

            public byte NextValue => (byte) (Sequence != null ? Sequence[Cursor] : 0);
        }

        private ServoMixerService _servoMixerService;

        [SerializeField] private ServoAnimation[] _servoActions;

        private void Start()
        {
            _servoMixerService = Robot.Instance.GetServiceOfType<ServoMixerService>();
        }

        public override void Initialize()
        {
            foreach (var action in _servoActions)
                StartCoroutine(PlayAnimationAction(action));
        }

        public override void Dispose()
        {
            StopAllCoroutines();
        }

        private IEnumerator PlayAnimationAction(ServoAnimation animation)
        {
            while (Started)
            {
                var value = animation.RandomRange
                    ? (byte) UnityRandom.Range(animation.Min, animation.Max)
                    : animation.NextValue;

                _servoMixerService.SetServoValueInServo(animation.Servo, value);

                yield return CoroutineFactory.WaitForSeconds(animation.Frequency);
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!Started || !Application.isPlaying) return;
            Dispose();
            Initialize();
        }
#endif
    }
}