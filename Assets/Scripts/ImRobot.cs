using System;
using Demonixis.InMoov.Chatbots;
using Demonixis.InMoov.Servos;
using Demonixis.InMoov.Speech;
using Demonixis.InMoov.Speech.Microsoft;
using UnityEngine;

namespace Demonixis.InMoov
{
    public class ImRobot : MonoBehaviour
    {
        // Human Understanding
        private ChatbotService _chatbotService;
        private SpeechSynthesisService _speechSynthesis;
        private VoiceRecognitionService _voiceRecognition;
        
        // Animation
        private ServoMixer _servoMixer;

        private void Start()
        {
#if UNITY_WINDOWS
            _voiceRecognition = new MSVoiceRecognitionService();
            _speechSynthesis = new MSSpeechSynthesisService();
#else
            _voiceRecognition = new VoiceRecognitionService();
            _speechSynthesis = new SpeechSynthesisService();
#endif
            _chatbotService = new ChatbotServiceProject();
            _servoMixer = new ServoMixer();

            _voiceRecognition.Initialize();
            _speechSynthesis.Initialize();
            _chatbotService.Initialize();

            _voiceRecognition.PhraseDetected += s =>
            {
                var response = _chatbotService.GetResponse(s);
                _speechSynthesis.Speak(string.IsNullOrEmpty(response) ? "I don't understand" : response);
            };
            
            _servoMixer.Initialize();
        }
    }
}