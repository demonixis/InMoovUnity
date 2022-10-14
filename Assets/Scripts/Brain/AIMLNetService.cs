using System;
using System.IO;
using AIMLbot;
using UnityEngine;

namespace Demonixis.InMoov.Chatbots
{
    public class AIMLNetService : ChatbotService
    {
        private const string UserId = "unity";
        private Bot _aimlBot;
        private User _user;
        private string _pathToUserSettings;

        public override void Initialize()
        {
            _aimlBot = new Bot();
            _user = new User(UserId, _aimlBot);

            if (!Application.isEditor)
            {
                _pathToUserSettings = Application.streamingAssetsPath + @"\Brain-Graphmaster.xml";
            }
            else
            {
                _pathToUserSettings = Application.persistentDataPath + @"\Brain-Graphmaster.xml";
            }
            
            _aimlBot.ChangeMyPath =  Application.streamingAssetsPath;
            _aimlBot.loadSettings();
            _aimlBot.isAcceptingUserInput = false;
            _aimlBot.loadAIMLFromFiles();
            _aimlBot.isAcceptingUserInput = true;
            
            LoadBrain();
            
            base.Initialize();
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
            SaveBrain();
            base.Shutdown();
        }
        
        public void SaveBrain()
        {
            try
            {
                _user.Predicates.DictionaryAsXML.Save(_pathToUserSettings);
                Debug.Log("Brain saved");
            }
            catch (Exception e)
            {
                Debug.Log("Brain not saved");
                Debug.Log(e);
            }
        }

        public void LoadBrain()
        {
            try
            {
                _user.Predicates.loadSettings(_pathToUserSettings);
                Debug.Log("Brain loaded");
            }
            catch (Exception e)
            {
                Debug.Log("Brain not loaded");
                Debug.Log(e);
            }
        }
    }
}