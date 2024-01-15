using Demonixis.InMoovSharp.Settings;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Demonixis.InMoovSharp.Services
{
    public sealed class BrainWorldContext : IDisposable
    {
        private ChatbotService _chatbot;
        private SpeechSynthesisService _speechSynthesis;
        private VoiceRecognitionService _voiceRecognition;

        [NonSerialized]
        public List<string> DetectedObjects = new();
        [NonSerialized]
        public List<string> DetectedPersons = new();

        public event Action<int, Vector3> NewPersonDetected;

        public void Setup(ChatbotService chatbotService, VoiceRecognitionService voiceRecognition,
            SpeechSynthesisService speechSynthesis)
        {
            _chatbot = chatbotService;
            _voiceRecognition = voiceRecognition;
            _speechSynthesis = speechSynthesis;

            _chatbot.ResponseReady += OnChatbotResponse;
            _voiceRecognition.PhraseDetected += OnVoiceRecognized;

            var settings = GlobalSettings.Get();
            SetLanguage(settings.Language);
        }

        public void SetLanguage(string language)
        {
            _chatbot.SetLanguage(language);
            _voiceRecognition.SetLanguage(language);
            _speechSynthesis.SetLanguage(language);
        }

        private void OnChatbotResponse(string response)
        {
            _speechSynthesis.Speak(string.IsNullOrEmpty(response) ? "I don't understand" : response);
        }

        private void OnVoiceRecognized(string phrase)
        {
            if (DetectedObjects.Count > 0 && CheckIfWhatDoYouSee(phrase))
            {
                var message = new StringBuilder();
                message.Append($"I can see {DetectedObjects.Count} objects. ");

                foreach (var item in DetectedObjects)
                    message.Append($"{item}. ");

                _chatbot.SubmitResponse(phrase, true);
                _chatbot.NotifyResponseReady(message.ToString());
            }
            else
                _chatbot.SubmitResponse(phrase);
        }

        public void Dispose()
        {
            _chatbot.ResponseReady -= OnChatbotResponse;
            _voiceRecognition.PhraseDetected -= OnVoiceRecognized;
        }

        // FIXME: Hardcoded
        private bool CheckIfWhatDoYouSee(string phrase)
        {
            if (GlobalSettings.Get().Language == "fr-FR")
            {
                return (phrase.Contains("que") || phrase.Contains("ce que"))
                    && (phrase.Contains("vois") || phrase.Contains("voyez"))
                    && (phrase.Contains("tu") || phrase.Contains("vous"));
            }

            return phrase.Contains("what") &&
                   phrase.Contains("you") &&
                   phrase.Contains("see");
        }
    }
}