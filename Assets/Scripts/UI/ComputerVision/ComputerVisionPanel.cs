using System.Collections;
using Demonixis.InMoov.ComputerVision;
using Demonixis.InMoov.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Demonixis.InMoov
{
    public class ComputerVisionPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown _leftEyeList;
        [SerializeField] private TMP_Dropdown _rightEyeList;
        [SerializeField] private TMP_Dropdown _filterList;

        [SerializeField] private RawImage _leftRawImage;
        [SerializeField] private RawImage _rightRawImage;

        private void Start()
        {
            _filterList.options.Clear();
            _filterList.options.Add(new TMP_Dropdown.OptionData("None"));
            _filterList.options.Add(new TMP_Dropdown.OptionData("YoloV4 Object Detection"));
            _filterList.onValueChanged.AddListener(i =>
            {
                var visualizer = GetComponentInChildren<YoloV4Visualizer>(true);

                if (i == 0)
                {
                    visualizer.enabled = false;
                    return;
                }

                if (visualizer.enabled) return;
                
                var cv = Robot.Instance.GetService<ComputerVisionService>();
                var texture = cv.GetWebCamTexture(true);
                if (texture == null) return;

                visualizer.Setup(texture, _leftRawImage);
                visualizer.enabled = true;
            });

            _leftEyeList.onValueChanged.AddListener(i => UpdateRawImage(true, _leftEyeList.value - 1));
            _rightEyeList.onValueChanged.AddListener(i => UpdateRawImage(false, _leftEyeList.value - 1));
        }

        private void UpdateRawImage(bool left, int index)
        {
            var cv = Robot.Instance.GetService<ComputerVisionService>();
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
            var cv = Robot.Instance.GetService<ComputerVisionService>();
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