#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
#define MS_SPEECH_SYNTHESIS
#endif

using System.Text;
#if MS_SPEECH_SYNTHESIS
using System.Runtime.InteropServices;
using UnityEngine.Windows.Speech;
using UnityEngine;
#endif

namespace Demonixis.InMoov.Speech
{
    public class WindowsSpeechSynthesis : SpeechSynthesisService
    {
        private bool _paused;
        
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

        public override void SetPaused(bool paused)
        {
            _paused = paused;
        }

        public override void Speak(string message)
        {
            if (!_paused)
                addToSpeechQueue(message);
        }

        public override void Shutdown()
        {
            destroySpeech();
        }
    }
}