using Demonixis.InMoovSharp.Services;
using Demonixis.InMoovSharp.Settings;
using UnityEngine;

namespace Demonixis.InMoovUnity.Services
{
    public partial class UnityComputerVision : ComputerVisionService
    {
        private const string CVFilename = "computer-vision.json";
        private CVSettings _settings;

        protected override void SafeInitialize()
        {
            base.SafeInitialize();

            _settings = SaveGame.LoadData<CVSettings>(CVFilename, "Config");

            if (!_settings.IsValid())
                return;

            if (_settings.IsCameraConnected(true))
                TryInitializeWebCamTexture(_settings.WebCamLeft, true, out WebCamTexture leftEye);

            if (_settings.IsCameraConnected(false))
                TryInitializeWebCamTexture(_settings.WebCamRight, false, out WebCamTexture rightEye);
        }

        public override void SetPaused(bool paused)
        {
        }

        public override void Dispose()
        {
            base.Dispose();

            if (_webCamTextures == null) return;

            foreach (var texture in _webCamTextures)
            {
                if (texture != null)
                    texture.Stop();
            }
        }
    }
}
