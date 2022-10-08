using System;
using Demonixis.InMoov.Chatbots;
using Demonixis.InMoov.Servos;
using Demonixis.InMoov.Speech;
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