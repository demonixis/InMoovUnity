using System.IO;
using AIMLbot;
using UnityEngine;

namespace Demonixis.InMoov.Chatbots
{
    public class ChatbotServiceProject : ChatbotService
    {
        private const string UserId = "unity";
        private Bot _aimlBot;
        private User _user;

        public override void Initialize()
        {
            var globalPath = $"{Application.streamingAssetsPath}/ChatbotProject";
            if (!Directory.Exists(globalPath))
            {
                Debug.LogException(
                    new UnityException($"The path {globalPath} doesn't exists! Can't initialize the Chatbot"));
            }

            _aimlBot = new Bot();
            _aimlBot.loadSettings(globalPath + "/config/Settings.xml");
            _aimlBot.isAcceptingUserInput = false;
            _aimlBot.loadAIMLFromFiles();
            _aimlBot.isAcceptingUserInput = true;

            _user = new User(UserId, _aimlBot);
        }

        public override void SetPaused(bool paused)
        {
            _aimlBot.isAcceptingUserInput = !paused;
        }

        public override string GetResponse(string speechInput)
        {
            var request = new Request(speechInput, _user, _aimlBot);
            var result = _aimlBot.Chat(request);
            return result.Output;
        }

        public override void Shutdown()
        {
        }
    }
}