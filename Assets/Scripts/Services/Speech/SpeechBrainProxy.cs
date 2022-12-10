using Demonixis.InMoov.Chatbots;

namespace Demonixis.InMoov.Services.Speech
{
    public sealed class SpeechBrainProxy
    {
        private ChatbotService _chatbot;
        private SpeechSynthesisService _speechSynthesis;
        private VoiceRecognitionService _voiceRecognition;
        
        public void Setup(ChatbotService chatbotService, VoiceRecognitionService voiceRecognition, SpeechSynthesisService speechSynthesis)
        {
            _chatbot = chatbotService;
            _voiceRecognition = voiceRecognition;
            _speechSynthesis = speechSynthesis;

            _chatbot.ResponseReady += OnChatbotResponse;
            _voiceRecognition.PhraseDetected += OnVoiceRecognized;
        }

        private void OnChatbotResponse(string response)
        {
            _speechSynthesis.Speak(string.IsNullOrEmpty(response) ? "I don't understand" : response);
        }

        private void OnVoiceRecognized(string phrase)
        {
            _chatbot.SubmitResponse(phrase);
        }

        private void Dispose()
        {
            _chatbot.ResponseReady -= OnChatbotResponse;
            _voiceRecognition.PhraseDetected -= OnVoiceRecognized;
        }
    }
}