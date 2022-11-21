using Demonixis.InMoov.Speech;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Demonixis.InMoov.Speech
{
    public class WindowsSpeechRecognitionWS : VoiceRecognitionService
    {
        public override void Initialize()
        {
            base.Initialize();
            SpeechLink.Instance.VoiceRecognized += Instance_VoiceRecognized;
        }

        private void Instance_VoiceRecognized(string obj)
        {
            NotifyPhraseDetected(obj);
        }
    }
}