using System;
using UnityEngine;

namespace Demonixis.InMoov.Chatbots
{
    public abstract class ChatbotService : RobotService
    {
        [SerializeField] private int _minWordsInPhrase = 1;
        [SerializeField] private int _minLettersInWord = 2;

        public event Action<string> ResponseReady;

        public abstract void SetLanguage(string culture);

        public abstract void SubmitResponse(string phrase, bool noReply = false);

        internal protected void NotifyResponseReady(string response)
        {
            ResponseReady?.Invoke(response);
        }

        public bool IsInputPhraseValid(string inputPhrase)
        {
            var tmp = inputPhrase.Split(' ');

            if (tmp.Length < _minLettersInWord ||
                tmp.Length == 0 && inputPhrase.Length < _minLettersInWord)
                return false;

            return true;
        }
    }
}