﻿using System;
using System.Collections;
using Demonixis.InMoov.Servos;
using Demonixis.InMoov.Utils;
using UnityEngine;
using UnityRandom = UnityEngine.Random;

namespace Demonixis.InMoov.Systems
{
    public class RandomAnimation : RobotSystem
    {
        [Serializable]
        public class RandomServoAction
        {
            private int _index;

            public ServoIdentifier Servo;
            public bool RandomRange;
            public byte Min;
            public byte Max;
            public byte[] Sequence;

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

        [SerializeField] private RandomServoAction[] _servoActions;
        [SerializeField] private float _frequency = 2.0f;

        protected override void Start()
        {
            _servoMixerService = Robot.Instance.GetServiceOfType<ServoMixerService>();
            base.Start();
        }
        
        public override void Initialize()
        {
            StartCoroutine(Loop());
        }

        public override void Dispose()
        {
            StopAllCoroutines();
        }

        private IEnumerator Loop()
        {
            Running = true;

            while (Running)
            {
                foreach (var action in _servoActions)
                {
                    var value = action.RandomRange
                        ? (byte) UnityRandom.Range(action.Min, action.Max)
                        : action.NextValue;

                    _servoMixerService.SetServoValueInServo(action.Servo, value);
                }

                yield return CoroutineFactory.WaitForSeconds(_frequency);
            }
        }
    }
}