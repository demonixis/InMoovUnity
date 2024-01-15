using System.Collections;
using System.Diagnostics;
using Demonixis.InMoov.Utils;
using UnityEngine;

namespace Demonixis.InMoov.Services.Speech
{
    public class MacosSpeechSynthesis : SpeechSynthesisService
    {
        public override RuntimePlatform[] SupportedPlateforms => new[]
        {
            RuntimePlatform.OSXEditor,
            RuntimePlatform.OSXPlayer
        };

        public override void Speak(string message)
        {
            if (Paused) return;
            StartCoroutine(SpeechLoop(message));
        }

        private IEnumerator SpeechLoop(string message)
        {
            NotifySpeechStarted(message);

            var waitTime = GetSpeakTime(message);
            Process.Start("say", message);

            yield return CoroutineFactory.WaitForSeconds(waitTime);

            NotifySpeechState(false);

            yield return CoroutineFactory.WaitForSeconds(DelayAfterSpeak);

            NotifySpeechState(true);
        }
    }
}