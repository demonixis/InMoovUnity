using Demonixis.InMoov.Settings;

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
                InitializeWebCamTexture(_settings.WebCamLeft, true);

            if (_settings.IsCameraConnected(false))
                InitializeWebCamTexture(_settings.WebCamRight, false);
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