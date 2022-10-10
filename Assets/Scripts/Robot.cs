using Demonixis.InMoov.Chatbots;
using Demonixis.InMoov.Servos;
using Demonixis.InMoov.Speech;
using UnityEngine;

namespace Demonixis.InMoov
{
    public class Robot : MonoBehaviour
    {
        private const string ServoMixerDataFilename = "servo.json";
        private const string SerialPortDataFilename = "serial.json";
        
        // Human Understanding
        [SerializeField] private ChatbotService _chatbotService;
        [SerializeField] private SpeechSynthesisService _speechSynthesis;
        [SerializeField] private VoiceRecognitionService _voiceRecognition;

        // Animation
        [SerializeField] private ServoMixerService _servoMixerService;

        private void Start()
        {
            _voiceRecognition.Initialize();
            _speechSynthesis.Initialize();
            _chatbotService.Initialize();

            _voiceRecognition.PhraseDetected += s =>
            {
                var response = _chatbotService.GetResponse(s);
                _speechSynthesis.Speak(string.IsNullOrEmpty(response) ? "I don't understand" : response);
            };

            _servoMixerService.Initialize();
        }
    }
}