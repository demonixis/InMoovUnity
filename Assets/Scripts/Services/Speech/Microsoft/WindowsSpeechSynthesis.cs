#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
#define MS_SPEECH_SYNTHESIS
#endif

using System.Collections;
using System.Text;
using Demonixis.InMoov.Utils;
using UnityEngine;

#if MS_SPEECH_SYNTHESIS
using System.Runtime.InteropServices;
#endif

namespace Demonixis.InMoov.Services.Speech
{
    public class WindowsSpeechSynthesis : SpeechSynthesisService
    {
#if MS_SPEECH_SYNTHESIS
        [DllImport("WindowsVoice")]
        public static extern void initSpeech();

        [DllImport("WindowsVoice")]
        public static extern void destroySpeech();

        [DllImport("WindowsVoice")]
        public static extern void addToSpeechQueue(string s);

        [DllImport("WindowsVoice")]
        public static extern void clearSpeechQueue();

        [DllImport("WindowsVoice")]
        public static extern void statusMessage(StringBuilder str, int length);
#else
        public static void initSpeech () {}
        public static void destroySpeech () {}
        public static void addToSpeechQueue (string s) {}
        public static void clearSpeechQueue () {}
        public static void statusMessage (StringBuilder str, int length) {}
#endif

        [SerializeField] private bool _logOutput = true;

        public override RuntimePlatform[] SupportedPlateforms => new[]
        {
            RuntimePlatform.WindowsEditor,
            RuntimePlatform.WindowsPlayer
        };

        public override void Initialize()
        {
            initSpeech();
            base.Initialize();
        }

        public override void Speak(string message)
        {
            if (Paused) return;

            addToSpeechQueue(message);
            StartCoroutine(SpeechLoop(message));

            if (_logOutput)
                Debug.Log(message);
        }

        public override void Shutdown()
        {
            destroySpeech();
        }

        private IEnumerator SpeechLoop(string message)
        {
            NotifySpeechState(true, message);
            
            yield return CoroutineFactory.WaitForSeconds(GetSpeakTime(message));
            
            NotifySpeechState(false, null);
        }
    }
}