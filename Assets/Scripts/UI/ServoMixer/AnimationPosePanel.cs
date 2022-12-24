using Demonixis.InMoov.Servos;
using System;
using TMPro;
using UnityEngine;

namespace Demonixis.InMoov.UI
{
    public enum HandGestures
    {
        Close,
        Open,
        Rock,
        Point,
        ThumbUp,
        ThumbIndexUp,
        Fuck,
        Grab,
        Bunny
    }

    public enum ArmGestures
    {
        Close,
        Point,
        Intermediate
    }

    public sealed class AnimationPosePanel : MonoBehaviour
    {
        private ServoMixerService _servoMixerService;

        [SerializeField] private Transform _leftHandGesturesContainer;
        [SerializeField] private GameObject _handGesturePrefab;


        private void Start()
        {
            Robot.Instance.WhenStarted(Initialize);
        }

        private void Initialize()
        {
            _servoMixerService = FindObjectOfType<ServoMixerService>();

            foreach (Transform child in _leftHandGesturesContainer)
                Destroy(child.gameObject);

            var names = Enum.GetNames(typeof(HandGestures));

            for (var i = 0; i < names.Length; i++)
            {
                var item = Instantiate(_handGesturePrefab, _leftHandGesturesContainer);
                ServoMixerPanel.ResetTransform(item.transform);

                var handGestureItem = item.AddComponent<HandGestureItem>();
                handGestureItem.Setup((HandGestures)i);
                handGestureItem.Clicked += OnHandGestureClicked; ;
            }
        }

        private void OnHandGestureClicked(HandGestures obj)
        {
            ApplyHandGesture(true, obj);
            ApplyHandGesture(false, obj);
        }

        public void ApplyHandGesture(bool left, HandGestures gesture)
        {
            switch (gesture)
            {
                case HandGestures.Close:
                    ApplyHandGesture(left, 1, 1, 1, 1, 1);
                    break;
                case HandGestures.Open:
                    ApplyHandGesture(left, 0, 0, 0, 0, 0);
                    break;
                case HandGestures.Point:
                    ApplyHandGesture(left, 1, 0, 1, 1, 1);
                    break;
                case HandGestures.Rock:
                    ApplyHandGesture(left, 0, 0, 1, 1, 0);
                    break;
                case HandGestures.ThumbUp:
                    ApplyHandGesture(left, 0, 1, 1, 1, 1);
                    break;
                case HandGestures.ThumbIndexUp:
                    ApplyHandGesture(left, 0, 0, 1, 1, 1);
                    break;
                case HandGestures.Fuck:
                    ApplyHandGesture(left, 1, 1, 0, 1, 1);
                    break;
                case HandGestures.Grab:
                    ApplyHandGesture(left, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f);
                    break;
                case HandGestures.Bunny:
                    ApplyHandGesture(left, 1, 0, 0, 1, 1);
                    break;
            }
        }

        private void ApplyHandGesture(bool left, float thumb, float index, float middle, float ring, float pinky)
        {
            var fingers = new[]
            {
                left ? ServoIdentifier.LeftFingerThumb : ServoIdentifier.RightFingerThumb,
                left ? ServoIdentifier.LeftFingerIndex : ServoIdentifier.RightFingerIndex,
                left ? ServoIdentifier.LeftFingerMiddle : ServoIdentifier.RightFingerMiddle,
                left ? ServoIdentifier.LeftFingerRing : ServoIdentifier.RightFingerRing,
                left ? ServoIdentifier.LeftFingerPinky : ServoIdentifier.RightFingerPinky
            };

            var values = new float [] { thumb, index, middle, ring, pinky };
            for (var i = 0; i < fingers.Length; i++)
                _servoMixerService.SetServoValueInServo(fingers[i], (byte)(values[i] * 180));
        }
    }
}