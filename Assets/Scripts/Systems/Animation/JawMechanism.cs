using System.Collections;
using Demonixis.InMoov.Chatbots;
using Demonixis.InMoov.Servos;
using UnityEngine;

namespace Demonixis.InMoov.Systems
{
    public class JawMechanism : RobotSystem
    {
        private ServoMixerService _servoMixerService;
        private bool _initialized;

        [SerializeField] private float _jawOpenSpeed = 0.5f;
        [SerializeField] private float _jawCloseSpeed = 0.35f;
        [SerializeField] private byte _jawAmplitude = 10;
        [SerializeField] private byte _wordsPerMinute = 100;

        protected override void Start()
        {
            _servoMixerService = Robot.Instance.GetServiceOfType<ServoMixerService>();
            base.Start();
        }

        private void OnEnable()
        {
            InternalInitialize();
        }

        private void OnDisable()
        {
            InternalShutdown();
        }

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
            if (_initialized || !Running) return;

            var robot = Robot.Instance;
            var chatbot = robot.GetServiceOfType<ChatbotService>();
            chatbot.ResponseReady += OnChatBotResponse;
            robot.ServiceChanged += OnRobotServiceChanged;

            _initialized = true;
        }

        private void InternalShutdown()
        {
            if (!_initialized || !Running) return;

            var robot = Robot.Instance;
            var chatbot = robot.GetServiceOfType<ChatbotService>();
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
            if (oldService is not ChatbotService) return;

            var oldChatbot = (ChatbotService) oldService;
            oldChatbot.ResponseReady -= OnChatBotResponse;

            var newChatbot = (ChatbotService) newService;
            newChatbot.ResponseReady += OnChatBotResponse;
        }

        private IEnumerator MoveJaw(string sentence)
        {
            var words = sentence.Split(' ');
            var time = words.Length * 60.0f / _wordsPerMinute;
            var data = _servoMixerService.GetServoData(ServoIdentifier.Jaw);
            var elapsedTime = 0.0f;

            _servoMixerService.SetServoValueInServo(ServoIdentifier.Jaw, data.Neutral);

            while (elapsedTime > time)
            {
                yield return StartCoroutine(OpenJaw(data));
                yield return StartCoroutine(CloseJaw(data));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        private IEnumerator OpenJaw(ServoData data)
        {
            var elapsedTime = 0.0f;
            var currentValue = data.Neutral;
            var targetValue = (byte) (currentValue + _jawAmplitude);

            _servoMixerService.SetServoValueInServo(ServoIdentifier.Jaw, currentValue);

            while (elapsedTime < _jawOpenSpeed)
            {
                currentValue = (byte) Mathf.Lerp(currentValue, targetValue, elapsedTime / targetValue);
                elapsedTime += Time.deltaTime;
                _servoMixerService.SetServoValueInServo(ServoIdentifier.Jaw, currentValue);
                yield return null;
            }

            _servoMixerService.SetServoValueInServo(ServoIdentifier.Jaw, targetValue);
        }

        private IEnumerator CloseJaw(ServoData data)
        {
            var elapsedTime = 0.0f;
            var currentValue = (byte) (data.Neutral + _jawAmplitude);
            var targetValue = data.Neutral;

            _servoMixerService.SetServoValueInServo(ServoIdentifier.Jaw, currentValue);

            while (elapsedTime < _jawCloseSpeed)
            {
                currentValue = (byte) Mathf.InverseLerp(currentValue, targetValue, elapsedTime / targetValue);
                elapsedTime += Time.deltaTime;
                _servoMixerService.SetServoValueInServo(ServoIdentifier.Jaw, currentValue);
                yield return null;
            }

            _servoMixerService.SetServoValueInServo(ServoIdentifier.Jaw, targetValue);
        }
    }
}
