using Demonixis.InMoov.Servos;
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
        private ServoIdentifier _currentServo;
        private ServoData _currentData;
        
        private void Start()
        {
            Robot.Instance.WhenStarted(Initialize);
        }

        private void Initialize()
        {
            _currentData = ServoData.New(ServoIdentifier.None);
            _servoMixerService = FindObjectOfType<ServoMixerService>();
        }

        public void ApplyHandGestion(bool left, HandGestures gesture)
        {
            switch (gesture)
            {
                case HandGestures.Close:
                    ApplyHandGestion(left, 0, 0, 0, 0, 0);
                    break;
                case HandGestures.Open:
                    ApplyHandGestion(left, 1, 1, 1, 1, 1);
                    break;
                case HandGestures.Point:
                    ApplyHandGestion(left, 0, 1, 0, 0, 0);
                    break;
                case HandGestures.Rock:
                    ApplyHandGestion(left, 1, 1, 0, 0, 1);
                    break;
                case HandGestures.ThumbUp:
                    ApplyHandGestion(left, 0, 1, 0, 0, 0);
                    break;
                case HandGestures.ThumbIndexUp:
                    ApplyHandGestion(left, 1, 1, 0, 0, 0);
                    break;
                case HandGestures.Fuck:
                    ApplyHandGestion(left, 0, 0, 1, 0, 0);
                    break;
                case HandGestures.Grab:
                    ApplyHandGestion(left, 0, 0, 1, 0, 0);
                    break;
                case HandGestures.Bunny:
                    ApplyHandGestion(left, 0, 1, 1, 0, 0);
                    break;
            }
        }

        private void ApplyHandGestion(bool left, byte thumb, byte index, byte middle, byte ring, byte pinky)
        {
            var fingers = new[]
            {
                left ? ServoIdentifier.LeftFingerThumb : ServoIdentifier.RightFingerThumb,
                left ? ServoIdentifier.LeftFingerIndex : ServoIdentifier.RightFingerIndex,
                left ? ServoIdentifier.LeftFingerMiddle : ServoIdentifier.RightFingerMiddle,
                left ? ServoIdentifier.LeftFingerRing : ServoIdentifier.RightFingerRing,
                left ? ServoIdentifier.LeftFingerPinky : ServoIdentifier.RightFingerPinky
            };

            var values = new byte[] {thumb, index, middle, ring, pinky};
            for (var i = 0; i < fingers.Length; i++)
                _servoMixerService.SetServoValueInServo(fingers[i], values[i]);
        }
    }
}