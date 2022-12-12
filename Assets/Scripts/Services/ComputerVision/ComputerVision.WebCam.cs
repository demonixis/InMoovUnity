using UnityEngine;

namespace Demonixis.InMoov.ComputerVision
{
    public partial class ComputerVisionService
    {
        private WebCamTexture[] _webCamTextures;
        
        public void SetWebCamTextureEnabled(bool left, bool start)
        {
            if (_webCamTextures == null ||
                left && _webCamTextures[0] == null ||
                !left && _webCamTextures[1] == null)
                return;

            var texture = _webCamTextures[left ? 0 : 1];

            if (start)
                texture.Play();
            else
                texture.Stop();
        }

        public WebCamTexture GetWebCamTexture(bool left)
        {
            return _webCamTextures[left ? 0 : 1];
        }

        public void InitializeWebCamTexture(int cameraIndex, bool left, bool autoPlay = false)
        {
            if (!TryGetWebcamInfos(cameraIndex, out string cameraName, out int width, out int height))
            {
                Debug.LogError($"Can't open the webcam {cameraIndex}");
                return;
            }

            var texture = new WebCamTexture(cameraName, width, height);
            if (texture == null)
            {
                Debug.LogError($"Can't create a webcam texture for the camera {cameraIndex}");
                return;
            }

            if (_webCamTextures == null)
                _webCamTextures = new WebCamTexture[2];

            _webCamTextures[left ? 0 : 1] = texture;
        }

        private bool TryGetWebcamInfos(int cameraIndex, out string cameraName, out int width, out int height)
        {
            cameraName = string.Empty;
            width = 0;
            height = 0;

            var devices = WebCamTexture.devices;

            if (devices.Length == 0 || devices.Length < cameraIndex) return false;

            var camera = devices[cameraIndex];
            var resolution = GetBestResolution(camera.availableResolutions);

            cameraName = camera.name;
            width = resolution.width;
            height = resolution.height;

            return true;
        }

        private Resolution GetBestResolution(Resolution[] resolutions)
        {
            if (resolutions == null || resolutions.Length == 0)
            {
                return new Resolution
                {
                    width = 640,
                    height = 480
                };
            }

            var bestResolution = new Resolution();

            foreach (var res in resolutions)
            {
                if (res.width <= bestResolution.width || res.height <= bestResolution.height) continue;
                bestResolution.width = res.width;
                bestResolution.height = res.height;
            }

            return bestResolution;
        }
    }
}