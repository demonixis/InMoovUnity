using System.Collections;
using Demonixis.InMoov.Chatbots;
using Demonixis.InMoov.Utils;
using UnityEngine;
using YoloV4Tiny;

namespace Demonixis.InMoov.ComputerVision.Filters
{
    public sealed class YoloV4Visualizer : MLVisualizer
    {
        private ChatbotService _chatbot;
        private ObjectDetector _detector;
        private readonly YoloV4Marker[] _markers = new YoloV4Marker[50];
        
        [SerializeField, Range(0, 1)] float _threshold = 0.5f;
        [SerializeField] ResourceSet _resources;
        [SerializeField] YoloV4Marker yoloV4MarkerPrefab = null;

        private void OnEnable()
        {
            _chatbot = Robot.Instance.GetService<ChatbotService>();
            _detector = new ObjectDetector(_resources);
            
            for (var i = 0; i < _markers.Length; i++)
                _markers[i] = Instantiate(yoloV4MarkerPrefab, _preview.transform);

            StartCoroutine(UpdateDetectedObjects());
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            _detector.Dispose();
            _chatbot.DetectedObjects.Clear();
            
            foreach (var marker in _markers)
                Destroy(marker);
        }

        private void LateUpdate()
        {
            _detector.ProcessImage(_source, _threshold);

            var i = 0;
            foreach (var d in _detector.Detections)
            {
                if (i == _markers.Length) break;
                _markers[i++].SetAttributes(d);
            }

            for (; i < _markers.Length; i++) _markers[i].Hide();

            _preview.texture = _source;
        }

        private IEnumerator UpdateDetectedObjects()
        {
            while (true)
            {
                _chatbot.DetectedObjects.Clear();

                foreach (var d in _detector.Detections)
                {
                    var objName = YoloV4Marker._labels[(int) d.classIndex];
                    _chatbot.DetectedObjects.Add($"A {objName}");
                }

                yield return CoroutineFactory.WaitForSeconds(1.0f);
            }
        }
    }
}