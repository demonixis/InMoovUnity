using AIMLbot;
using Demonixis.InMoov.Settings;
using System;
using System.IO;
using UnityEngine;

namespace Demonixis.InMoov.Chatbots
{
    public class AIMLNetService : ChatbotService
    {
        [Serializable]
        public struct AIMLNetServiceData
        {
            public string User;
            public string Language;

            public bool IsValid()
            {
                return !string.IsNullOrEmpty(User) && !string.IsNullOrEmpty(Language);
            }

            public static AIMLNetServiceData CreateNew()
            {
                return new AIMLNetServiceData
                {
                    User = "Unity",
                    Language = "en-US"
                };
            }
        }

        private const string SerialFilename = "aiml-data.json";
        private Bot _aimlBot;
        private User _user;
        private AIMLNetServiceData _data;
        private string _pathToUserSettings;

        public override void Initialize()
        {
            _data = SaveGame.LoadRawData<AIMLNetServiceData>(SaveGame.GetPreferredStorageMode(), SerialFilename,
                "Brain");

            if (!_data.IsValid())
                _data = AIMLNetServiceData.CreateNew();

            InitializeBrain();
            LoadBrain();

            base.Initialize();
        }

        private void InitializeBrain()
        {
            _aimlBot = new Bot();
            _user = new User(_data.User, _aimlBot);

            _pathToUserSettings = Path.Combine(SaveGame.GetSavePath("Brain"), $"aiml-{_data.Language}-graphmaster.xml");

            _aimlBot.CustomResourcePath = Path.Combine(Application.streamingAssetsPath, "AIML.Net", _data.Language);
            _aimlBot.LoadSettings();
            _aimlBot.isAcceptingUserInput = false;
            _aimlBot.LoadAimlFromFiles();
            _aimlBot.isAcceptingUserInput = true;
        }

        public override void SetCulture(string culture)
        {
            if (!IsLanguageSupported(culture))
            {
                Debug.LogError($"[AIML.Net] Language {culture} is not supported.");
                return;
            }

            _data.Language = culture;
            Initialize();
        }

        private bool IsLanguageSupported(string lang)
        {
            return Directory.Exists(Path.Combine(Application.streamingAssetsPath, "AIML.Net", lang));
        }

        protected override void SubmitResponseToBot(string inputSpeech)
        {
            if (Paused) return;

            var request = new Request(inputSpeech, _user, _aimlBot);
            var result = _aimlBot.Chat(request);

            NotifyResponseReady(result.Output);
        }

        public override void Shutdown()
        {
            SaveGame.SaveRawData(SaveGame.GetPreferredStorageMode(), _data, SerialFilename, "Brain");
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
                _user.Predicates.LoadSettings(_pathToUserSettings);
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