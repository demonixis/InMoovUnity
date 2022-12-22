using System.Collections;
using Demonixis.InMoov.Utils;
using UnityEngine;

namespace Demonixis.InMoov.Services.Speech
{
    public class WindowsSpeechSynthesisWS : SpeechSynthesisService
    {
        private SpeechLink _speechLink;

        public override RuntimePlatform[] SupportedPlateforms => new[]
        {
            RuntimePlatform.WindowsEditor,
            RuntimePlatform.WindowsPlayer
        };

        public override void Initialize()
        {
            base.Initialize();
            _speechLink = SpeechLink.Instance;
        }

        public override void SetVoice(int voiceIndex)
        {
            _speechLink.SetVoice(voiceIndex);
        }

        public override void SetVoice(string voiceName)
        {
            _speechLink.SetVoice(voiceName);
        }

        public override string[] GetVoices()
        {
            return _speechLink.Voices;
        }

        public override int GetVoiceIndex()
        {
            return _speechLink.VoiceIndex;
        }

        public override void Speak(string message)
        {
            if (Paused) return;
            _speechLink.Speak(message);
            StartCoroutine(SpeechLoop(message));
        }

        private IEnumerator SpeechLoop(string message)
        {
            NotifySpeechState(true, message);

            while (_speechLink.IsSpeaking)
                yield return null;

            yield return CoroutineFactory.WaitForSeconds(1.0f);

            NotifySpeechState(false, null);
        }
    }
}