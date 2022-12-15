using MediaPipe.BlazeFace;
using UnityEngine;

namespace Demonixis.InMoov.ComputerVision.Filters
{
    public sealed class BlazeFaceVisualizer : MLVisualizer
    {
        private FaceDetector _detector;
        private BlazeFaceMarker[] _markers = new BlazeFaceMarker[16];
        
        [SerializeField, Range(0, 1)] private float _threshold = 0.75f;
        [SerializeField] private ResourceSet _resources;
        [SerializeField] private BlazeFaceMarker blazeFaceMarkerPrefab;

        private void OnEnable()
        {
            _detector = new FaceDetector(_resources);
            
            for (var i = 0; i < _markers.Length; i++)
                _markers[i] = Instantiate(blazeFaceMarkerPrefab, _preview.transform);
        }

        private void OnDestroy()
        {
            _detector?.Dispose();

            foreach (var marker in _markers)
                Destroy(marker);
        }

        private void LateUpdate()
        {
            // Face Detection
            _detector.ProcessImage(_source, _threshold);

            // BlazeFaceMarker update
            var i = 0;

            foreach (var detection in _detector.Detections)
            {
                if (i == _markers.Length) break;
                var marker = _markers[i++];
                marker.Detection = detection;
                marker.gameObject.SetActive(true);
            }

            for (; i < _markers.Length; i++)
                _markers[i].gameObject.SetActive(false);

            // UI update
            _preview.texture = _source;
        }
    }
}