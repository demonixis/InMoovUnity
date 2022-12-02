using UnityEngine;
using Yetibyte.Unity.SpeechRecognition;

namespace Demonixis.InMoov.Services.Speech
{
    [RequireComponent(typeof(VoskListener))]
    public class VoskVoiceRecognitionService : VoiceRecognitionService
    {
        public override RuntimePlatform[] SupportedPlateforms => new[]
        {
            RuntimePlatform.LinuxEditor,
            RuntimePlatform.LinuxPlayer,
            RuntimePlatform.WindowsEditor,
            RuntimePlatform.WindowsPlayer,
            RuntimePlatform.OSXEditor,
            RuntimePlatform.OSXPlayer
        };

        public override void Initialize()
        {
            base.Initialize();

            var vosk = GetComponent<VoskListener>();
            vosk.LoadModel();
            vosk.StartListening();
            vosk.ResultFound += Vosk_ResultFound;
        }

        private void Vosk_ResultFound(object sender, VoskResultEventArgs e)
        {
            NotifyPhraseDetected(e.Result.Text);
        }

        public override void Shutdown()
        {
            var vosk = GetComponent<VoskListener>();
            vosk.StopListening();
            vosk.ResultFound -= Vosk_ResultFound;
        }
    }
}