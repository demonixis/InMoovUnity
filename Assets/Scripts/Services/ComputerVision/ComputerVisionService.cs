using Demonixis.InMoov.Settings;
using UnityEngine;

namespace Demonixis.InMoov.ComputerVision
{
    public partial class ComputerVisionService : RobotService
    {
        private const string CVFilename = "computer-vision.json";
        private CVSettings _settings;

        public override void Initialize()
        {
            base.Initialize();

            _settings = SaveGame.LoadRawData<CVSettings>(SaveGame.GetPreferredStorageMode(), CVFilename, "Config");

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

        public override void Shutdown()
        {
            base.Shutdown();

            if (_webCamTextures == null) return;

            foreach (var texture in _webCamTextures)
            {
                if (texture != null)
                    texture.Stop();
            }
        }
    }
}