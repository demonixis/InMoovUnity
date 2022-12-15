using UnityEngine;
using UnityEngine.UI;

namespace Demonixis.InMoov.ComputerVision.Filters
{
    public sealed class BlazeFaceKeyPointsUpdater : MonoBehaviour
    {
        private BlazeFaceMarker _blazeFaceMarker;
        private RectTransform _xform;
        private RectTransform _parent;
        private Text _label;
        
        [SerializeField] RectTransform[] _keyPoints;

        private void Start()
        {
            _blazeFaceMarker = GetComponent<BlazeFaceMarker>();
            _xform = GetComponent<RectTransform>();
            _parent = (RectTransform) _xform.parent;
            _label = GetComponentInChildren<Text>();
        }

        private void LateUpdate()
        {
            var detection = _blazeFaceMarker.Detection;

            // Bounding box center
            _xform.anchoredPosition = detection.center * _parent.rect.size;

            // Bounding box size
            var size = detection.extent * _parent.rect.size;
            _xform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
            _xform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);

            // Key points
            SetKeyPoint(_keyPoints[0], detection.leftEye);
            SetKeyPoint(_keyPoints[1], detection.rightEye);
            SetKeyPoint(_keyPoints[2], detection.nose);
            SetKeyPoint(_keyPoints[3], detection.mouth);
            SetKeyPoint(_keyPoints[4], detection.leftEar);
            SetKeyPoint(_keyPoints[5], detection.rightEar);

            // Label
            _label.text = $"{(int) (detection.score * 100)}%";
        }

        private void SetKeyPoint(RectTransform xform, Vector2 point)
        {
            xform.anchoredPosition = point * _parent.rect.size - _xform.anchoredPosition;
        }
    }
}