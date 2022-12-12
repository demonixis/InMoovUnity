using Demonixis.InMoov.Chatbots;
using Demonixis.InMoov.Servos;
using System.Collections;
using Demonixis.InMoov.Utils;
using UnityEngine;

namespace Demonixis.InMoov.Systems
{
    public class JawMechanism : RobotSystem
    {
        private ServoMixerService _servoMixerService;
        private bool _initialized;

        [SerializeField] private float _jawOpenTime = 0.25f;
        [SerializeField] private float _jawCloseTime = 0.15f;
        [SerializeField] private byte _jawAmplitude = 25;
        [SerializeField] private byte _wordsPerMinute = 40;
        
        public override void Initialize()
        {
            InternalInitialize();
        }

        public override void Dispose()
        {
            InternalShutdown();
        }

        private void InternalInitialize()
        {
            if (_initialized) return;


            var robot = Robot.Instance;
            _servoMixerService = robot.GetService<ServoMixerService>();

            var chatbot = robot.GetService<ChatbotService>();
            chatbot.ResponseReady += OnChatBotResponse;
            robot.ServiceChanged += OnRobotServiceChanged;

            _initialized = true;
        }

        private void InternalShutdown()
        {
            if (!_initialized) return;

            var robot = Robot.Instance;
            var chatbot = robot.GetService<ChatbotService>();
            if (chatbot == null) return;
            chatbot.ResponseReady -= OnChatBotResponse;
            robot.ServiceChanged -= OnRobotServiceChanged;

            _initialized = false;
        }

        private void OnChatBotResponse(string response)
        {
            StopAllCoroutines();
            StartCoroutine(MoveJaw(response));
        }

        private void OnRobotServiceChanged(RobotService oldService, RobotService newService)
        {
            if (oldService is not ChatbotService oldChatbot) return;

            oldChatbot.ResponseReady -= OnChatBotResponse;

            var newChatbot = (ChatbotService) newService;
            newChatbot.ResponseReady += OnChatBotResponse;
        }

        private IEnumerator MoveJaw(string sentence)
        {
            var words = sentence.Split(' ');
            var targetTime = words.Length * 60.0f / _wordsPerMinute;
            var data = _servoMixerService.GetServoData(ServoIdentifier.Jaw);
            var elapsedTime = 0.0f;
            var openMouthValue = (byte) (data.Neutral + _jawAmplitude);
            var startTime = Time.time;

            _servoMixerService.SetServoValueInServo(ServoIdentifier.Jaw, data.Neutral);

            while (elapsedTime < targetTime)
            {
                _servoMixerService.SetServoValueInServo(ServoIdentifier.Jaw, openMouthValue);
                yield return CoroutineFactory.WaitForSeconds(_jawOpenTime);
                _servoMixerService.SetServoValueInServo(ServoIdentifier.Jaw, data.Neutral);
                yield return CoroutineFactory.WaitForSeconds(_jawCloseTime);
                elapsedTime += Time.time - startTime;
            }
        }
    }
}