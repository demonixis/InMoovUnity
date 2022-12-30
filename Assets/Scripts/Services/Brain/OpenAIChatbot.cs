using Demonixis.InMoov.Settings;
using System;
using AIMLbot.AIMLTagHandlers;
using OpenAI;
using UnityEngine;

namespace Demonixis.InMoov.Chatbots
{
    public class OpenAIChatbot : ChatbotService
    {
        private OpenAIAPI _openAIAPI;

        private Engine _engine = Engine.Davinci;

        [SerializeField] public int _maxToken = 50;

        public override void Initialize()
        {
            var settings = GlobalSettings.Get();
            var key = settings.OpenAIKey;

            if (string.IsNullOrEmpty(key))
            {
                Debug.LogError("The API key for OpenAI is missing.");
                return;
            }

            _openAIAPI = new OpenAIAPI(settings.OpenAIKey, _engine);

            base.Initialize();
        }

        public override void SetLanguage(string culture)
        {
        }

        public override async void SubmitResponse(string inputSpeech, bool noReply = false)
        {
            if (Paused) return;

            var request = new CompletionRequestBuilder()
                .WithPrompt(inputSpeech)
                .WithMaxTokens(_maxToken)
                .Build();

            var result = await _openAIAPI.Completions.CreateCompletionAsync(request);

            if (!noReply)
                NotifyResponseReady(result.ToString());
        }
    }
}