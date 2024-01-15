#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
#define MS_SPEECH_SYNTHESIS
#endif

using Demonixis.InMoovSharp.Services;
using Demonixis.InMoovSharp.Utils;
using System.Collections;
using UnityEngine;

namespace Demonixis.InMoovUnity.Services
{
    public class MicrosoftTTS : SpeechSynthesisService
    {
#if MS_SPEECH_SYNTHESIS
        private SpeechLink _speechLink;
#endif
        public override RuntimePlatform[] SupportedPlateforms => new[]
        {
            RuntimePlatform.WindowsEditor,
            RuntimePlatform.WindowsPlayer
        };

#if MS_SPEECH_SYNTHESIS
        public override void Initialize()
        {
            base.Initialize();
            _speechLink = SpeechLink.Instance;
            _speechLink.Initialize();
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
            NotifySpeechStarted(message);

            while (_speechLink.IsSpeaking)
                yield return null;

            NotifySpeechState(false);

            yield return CoroutineFactory.WaitForSeconds(DelayAfterSpeak);

            NotifySpeechState(true);
        }
#endif
    }
}
