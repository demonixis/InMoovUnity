using Demonixis.InMoov.Settings;
using System;
using UnityEngine;

namespace Demonixis.InMoov.Chatbots
{
    public class OpenAIChatbot : ChatbotService
    {
        public override void Initialize()
        {
     

            base.Initialize();
        }

        public override void SetCulture(string culture)
        {

        }

        protected override void SubmitResponseToBot(string inputSpeech)
        {
            if (Paused) return;
        }
    }
}