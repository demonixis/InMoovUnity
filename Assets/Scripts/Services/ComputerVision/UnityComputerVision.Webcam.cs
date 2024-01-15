using UnityEngine;

namespace Demonixis.InMoovUnity.Services
{
	public partial class UnityComputerVision
    {
		private WebCamTexture[] _webCamTextures = new WebCamTexture[2];

		public void SetWebCamTextureEnabled(bool left, bool start)
		{
			var texture = GetWebCamTexture(left);
			if (texture == null) return;

			if (start)
				texture.Play();
			else
				texture.Stop();
		}

		public void ToggleWebCamTexturePlayer(bool left)
		{
			var texture = GetWebCamTexture(left);
			if (texture == null) return;

			if (texture.isPlaying)
				texture.Stop();
			else
				texture.Play();
		}

		public WebCamTexture GetWebCamTexture(bool left)
		{
			return _webCamTextures[left ? 0 : 1];
		}

		public bool TryInitializeWebCamTexture(int cameraIndex, bool left, out WebCamTexture texture)
		{
			if (!TryGetWebcamInfos(cameraIndex, out string cameraName, out int width, out int height))
			{
				Debug.LogError($"Can't open the webcam {cameraIndex}");
				texture = null;
				return false;
			}

			texture = new WebCamTexture(cameraName, width, height);

			if (_webCamTextures == null)
				_webCamTextures = new WebCamTexture[2];

			var index = left ? 0 : 1;

			if (_webCamTextures[index] != null && _webCamTextures[index].isPlaying)
			{
				_webCamTextures[index].Stop();
				_webCamTextures[index] = null;
			}

			_webCamTextures[index] = texture;

			return texture;
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