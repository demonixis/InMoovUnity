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
        [SerializeField] private bool _logOutput = true;

        public override RuntimePlatform[] SupportedPlateforms => new[]
        {
            RuntimePlatform.WindowsEditor,
            RuntimePlatform.WindowsPlayer,
            RuntimePlatform.WSAPlayerX64
        };
        
#if MS_SPEECH_SYNTHESIS

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
            NotifySpeechStarted(message);

            var waitTime = GetSpeakTime(message);

            NotifySpeechState(false);

            yield return CoroutineFactory.WaitForSeconds(waitTime + 1.0f);

            NotifySpeechState(true);
        }
        
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

        [DllImport("WindowsVoice")]
        public static extern void changeVoice(int vIdx);

        [DllImport("WindowsVoice")]
        public static extern bool isSpeaking();
#endif
    }
}