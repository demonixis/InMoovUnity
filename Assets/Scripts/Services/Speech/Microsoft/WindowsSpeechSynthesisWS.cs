using System.Collections;
using System.Collections.Generic;
using Demonixis.InMoov.Utils;
using UnityEngine;

namespace Demonixis.InMoov.Services.Speech
{
    public class WindowsSpeechSynthesisWS : SpeechSynthesisService
    {
        public override RuntimePlatform[] SupportedPlateforms => new[]
        {
            RuntimePlatform.WindowsEditor,
            RuntimePlatform.WindowsPlayer
        };

        public override void SetVoice(int voiceIndex)
        {
            SpeechLink.Instance.SetVoice(voiceIndex);
        }

        public override void SetVoice(string voiceName)
        {
            SpeechLink.Instance.SetVoice(voiceName);
        }

        public override string[] GetVoices()
        {
            return SpeechLink.Instance.Voices;
        }

        public override void Speak(string message)
        {
            if (Paused) return;
            SpeechLink.Instance.Speak(message);
            StartCoroutine(SpeechLoop(message));
        }
        
        private IEnumerator SpeechLoop(string message)
        {
            NotifySpeechState(true, message);

            var waitTime = GetSpeakTime(message);
            yield return CoroutineFactory.WaitForSeconds(waitTime);
            
            NotifySpeechState(false, null);
        }
    }
}
