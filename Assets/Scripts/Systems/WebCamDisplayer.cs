using UnityEngine;
using UnityEngine.UI;

namespace Demonixis.InMoov
{
    public class WebCamDisplayer : MonoBehaviour
    {
        private WebCamTexture _webCamTexture;

        [SerializeField] private RawImage _rawimage;

        public void SetActive(bool active, int cameraIndex)
        {
            if (_webCamTexture == null) return;

            if (!active)
            {
                _webCamTexture.Stop();
                _webCamTexture = null;
                _rawimage.enabled = false;
            }
            else
                ActivateWebcamTexture(cameraIndex);
        }

        private void ActivateWebcamTexture(int cameraIndex)
        {

            if (!TryGetWebcamInfos(cameraIndex, out string cameraName, out int width, out int height, out int refreshRate))
            {
                Debug.LogError($"Can't open the webcam {cameraIndex}");
                _rawimage.enabled = false;
                return;
            }

            _webCamTexture = new WebCamTexture(cameraName, width, height, refreshRate);

            if (_webCamTexture == null)
            {
                Debug.LogError($"Can't create a webcam texture for the camera {cameraIndex}");
                return;
            }

            _rawimage.texture = _webCamTexture;
            _rawimage.enabled = true;
            _webCamTexture.Play();
        }

        private bool TryGetWebcamInfos(int cameraIndex, out string cameraName, out int width, out int height, out int refreshRate)
        {
            cameraName = string.Empty;
            width = 0;
            height = 0;
            refreshRate = 0;

            var devices = WebCamTexture.devices;

            if (devices.Length == 0 || devices.Length < cameraIndex) return false;

            var camera = devices[cameraIndex];
            var resolution = GetBestResolution(camera.availableResolutions);

            cameraName = camera.name;
            width = resolution.width;
            height = resolution.height;
            refreshRate = resolution.refreshRate;

            return true;
        }

        private Resolution GetBestResolution(Resolution[] resolutions)
        {
            if (resolutions == null || resolutions.Length == 0)
            {
                return new Resolution
                {
                    width = 640,
                    height = 480,
                    refreshRate = 30
                };
            }

            var bestResolution = new Resolution();

            foreach (var res in resolutions)
            {
                if (res.width > bestResolution.width && res.height > bestResolution.height && res.refreshRate > bestResolution.refreshRate)
                {
                    bestResolution.width = res.width;
                    bestResolution.height = res.height;
                    bestResolution.refreshRate = res.refreshRate;
                }
            }

            return bestResolution;
        }
    }
}