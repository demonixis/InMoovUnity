using Demonixis.InMoov.Chatbots;
using Demonixis.InMoov.Servos;
using System.Collections;
using Demonixis.InMoov.Services.Speech;
using Demonixis.InMoov.Utils;
using UnityEngine;

namespace Demonixis.InMoov.Systems
{
    public class JawMechanism : RobotSystem
    {
        private ServoMixerService _servoMixerService;
        private bool _initialized;
        private string _phraseTarget;
        private bool _isSpeaking;

        [SerializeField] private float _jawOpenTime = 0.25f;
        [SerializeField] private float _jawCloseTime = 0.15f;
        [SerializeField] private byte _jawAmplitude = 40;
        [SerializeField] private byte _jawNeutralOffset = 10;

        public override void Initialize()
        {
            InternalInitialize();
        }
        
        private void InternalInitialize()
        {
            base.Initialize();
            
            if (_initialized) return;
            
            var robot = Robot.Instance;
            _servoMixerService = robot.GetService<ServoMixerService>();

            var speechSynthesis = robot.GetService<SpeechSynthesisService>();
            speechSynthesis.SpeechStarted += SpeechSynthesisOnSpeechStarted;
            speechSynthesis.SpeechFinished += SpeechSynthesisOnSpeechFinished;
            
            _initialized = true;
        }

        private void SpeechSynthesisOnSpeechStarted(string message)
        {
            if (_isSpeaking)
            {
                StopAllCoroutines();
            }
            
            StartCoroutine(MoveJaw(message));
        }

        private void SpeechSynthesisOnSpeechFinished()
        {
            _isSpeaking = false;
        }

        private IEnumerator MoveJaw(string sentence)
        {
            if (string.IsNullOrEmpty(sentence)) yield break;

            _isSpeaking = true;
            
            var data = _servoMixerService.GetServoData(ServoIdentifier.Jaw);
            var openMouthValue = (byte) (data.Neutral + _jawAmplitude);
            var closeMouthValue = (byte) (data.Neutral + _jawNeutralOffset);

            _servoMixerService.SetServoValueInServo(ServoIdentifier.Jaw, data.Neutral);

            while (_isSpeaking)
            {
                _servoMixerService.SetServoValueInServo(ServoIdentifier.Jaw, openMouthValue);
                yield return CoroutineFactory.WaitForSeconds(_jawOpenTime);
                _servoMixerService.SetServoValueInServo(ServoIdentifier.Jaw, closeMouthValue);
                yield return CoroutineFactory.WaitForSeconds(_jawCloseTime);
            }
            
            _servoMixerService.SetServoValueInServo(ServoIdentifier.Jaw, data.Neutral);
        }
    }
}