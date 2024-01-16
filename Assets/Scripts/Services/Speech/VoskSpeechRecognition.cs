using Demonixis.InMoovSharp.Services;
using System.IO;
using UnityEngine;
using Yetibyte.Unity.SpeechRecognition;

namespace Demonixis.InMoovUnity.Services
{
    public class VoskSpeechRecognition : VoiceRecognitionService
    {
        private VoskListener _voskListener;
        private string _pendingLanguage;
        
        public override RuntimePlatform[] SupportedPlateforms => new[]
        {
            RuntimePlatform.Android,
            RuntimePlatform.LinuxPlayer,
            RuntimePlatform.LinuxEditor,
            RuntimePlatform.WindowsPlayer,
            RuntimePlatform.WindowsEditor,
            RuntimePlatform.OSXPlayer,
            RuntimePlatform.OSXEditor
        };

        protected override void SafeInitialize()
        {
            base.SafeInitialize();

            GetVoskListener();

            _voskListener.LoadModel();
            _voskListener.StartListening();
            _voskListener.ResultFound += Vosk_ResultFound;

            if (string.IsNullOrEmpty(_pendingLanguage))
                return;

            SetLanguage(_pendingLanguage);
            _pendingLanguage = null;
        }

        private void GetVoskListener()
        {
            _voskListener = Object.FindAnyObjectByType<VoskListener>();

            if (_voskListener != null)
                return;
            
            Debug.LogError("VoskListener doesn't exists! Creating it");
            UnityRobotProxy.Instance.gameObject.AddComponent<VoskListener>();
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
            // If not yet ready
            if (_voskListener == null)
            {
                _pendingLanguage = langPattern;
                return;
            }

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

        public override void Dispose()
        {
            base.Dispose();

            if (_voskListener == null)
                return;

            _voskListener.StopListening();
            _voskListener.ResultFound -= Vosk_ResultFound;
            _voskListener = null;
        }
    }
}