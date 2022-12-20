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

        protected override async void SubmitResponseToBot(string inputSpeech)
        {
            if (Paused) return;

            var request = new CompletionRequestBuilder()
                .WithPrompt(inputSpeech)
                .WithMaxTokens(50)
                .Build();

            var result = await _openAIAPI.Completions.CreateCompletionAsync(request);

            NotifyResponseReady(result.ToString());
        }
    }
}