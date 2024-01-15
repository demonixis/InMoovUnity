using System.IO;
using UnityEngine;
using Yetibyte.Unity.SpeechRecognition;

namespace Demonixis.InMoov.Services.Speech
{
    [RequireComponent(typeof(VoskListener))]
    public sealed class VoskVoiceRecognitionService : VoiceRecognitionService
    {
        private VoskListener _voskListener;

        public override void Initialize()
        {
            base.Initialize();

            _voskListener = GetComponent<VoskListener>();
            _voskListener.LoadModel();
            _voskListener.StartListening();
            _voskListener.ResultFound += Vosk_ResultFound;
        }

        public override void SetLanguage(string culture)
        {
            if (culture.Contains("fr-"))
                TryLoadModel("-fr-");
            else if (culture.Contains("en-"))
                TryLoadModel("-en-");
        }

        private void TryLoadModel(string langPattern)
        {
            var model = FindVoskModel(langPattern);
            if (model == null) return;
            _voskListener.ReloadModel(model);
        }

        private string FindVoskModel(string langPattern)
        {
            var voskPath = $"{Application.streamingAssetsPath}/VoskModels/";
            var voskModels = Directory.GetDirectories(voskPath);

            foreach (var model in voskModels)
            {
                if (model.Contains(langPattern))
                    return model;
            }

            return null;
        }

        private void SetListening(bool listening)
        {
            if (listening)
                _voskListener.StartListening();
            else
                _voskListener.StopListening();

            Debug.Log($"[Vosk] Listening: {listening}");
        }

        public override void SetPaused(bool paused)
        {
            base.SetPaused(paused);
            SetListening(!paused);
        }

        protected override void SetLocked(bool locked)
        {
            base.SetLocked(locked);

            if (!Paused)
                SetListening(!locked);
        }

        private void Vosk_ResultFound(object sender, VoskResultEventArgs e)
        {
            if (!CanListen)
            {
                Debug.Log("Can't listen");
                return;
            }

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