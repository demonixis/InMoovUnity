using Demonixis.InMoovSharp.Utils;
using Demonixis.InMoovUnity;
using Demonixis.InMoovUnity.Services;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Demonixis.InMoov
{
    public class ComputerVisionPanel : MonoBehaviour
    {
        [Header("Webcams")] [SerializeField] private TMP_Dropdown _leftEyeList;
        [SerializeField] private TMP_Dropdown _rightEyeList;

        [Header("Webcam previews")] [SerializeField]
        private RawImage _leftRawImage;

        [SerializeField] private RawImage _rightRawImage;

        [Header("ML Filters")] [SerializeField]
        private Toggle _yoloObjectDetector;

        [SerializeField] private Toggle _blazeFaceDetector;

        private void Start()
        {
            _leftEyeList.onValueChanged.AddListener(i => UpdateRawImage(true, _leftEyeList.value - 1));
            _rightEyeList.onValueChanged.AddListener(i => UpdateRawImage(false, _leftEyeList.value - 1));
        }

        private void UpdateRawImage(bool left, int index)
        {
            var robot = UnityRobotProxy.Instance.Robot;
            var cv = robot.GetService<UnityComputerVision>();
            var rawImage = left ? _leftRawImage : _rightRawImage;

            if (index < 0)
            {
                var current = cv.GetWebCamTexture(left);
                if (current != null)
                    current.Stop();

                rawImage.texture = null;
            }
            else
            {
                if (!cv.TryInitializeWebCamTexture(index, left, out WebCamTexture texture)) return;
                rawImage.texture = texture;
            }
        }

        public void PlayTexture(bool left)
        {
            var robot = UnityRobotProxy.Instance.Robot;
            var cv = robot.GetService<UnityComputerVision>();
            cv.ToggleWebCamTexturePlayer(left);
        }

        private void OnEnable()
        {
            StartCoroutine(UpdateVideoSources());
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private IEnumerator UpdateVideoSources()
        {
            while (true)
            {
                var sources = WebCamTexture.devices;

                if (_leftEyeList.options.Count != sources.Length + 1)
                {
                    _leftEyeList.options.Clear();
                    _rightEyeList.options.Clear();

                    _leftEyeList.options.Add(new TMP_Dropdown.OptionData("None"));
                    _rightEyeList.options.Add(new TMP_Dropdown.OptionData("None"));

                    foreach (var device in sources)
                    {
                        _leftEyeList.options.Add(new TMP_Dropdown.OptionData(device.name));
                        _rightEyeList.options.Add(new TMP_Dropdown.OptionData(device.name));
                    }
                }

                yield return CoroutineFactory.WaitForSeconds(1.5f);
            }
        }
    }
}