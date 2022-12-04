using Demonixis.InMoov.Servos;
using Demonixis.InMoov.Utils;
using System.Collections;
using UnityEngine;

namespace Demonixis.InMoov.Systems
{
    public class MecanimToServo : RobotSystem
    {
        private ServoMixerService _servoMixerService;

        [Header("Config")][SerializeField] private float _updateInterval = 1.0f / 30.0f;

        [Header("Rig")][SerializeField] private Transform _rig;
        [SerializeField] private Transform _head;
        [SerializeField] private Transform _neck;
        [SerializeField] private Transform _hip;

        [Header("Left (Top)")]
        [SerializeField]
        private Transform _leftShoulder;

        [SerializeField] private Transform _leftUpperArm;
        [SerializeField] private Transform _leftForeArm;
        [SerializeField] private Transform _leftWrist;
        [SerializeField] private Transform _leftThumbFinger;
        [SerializeField] private Transform _leftIndexFinger;
        [SerializeField] private Transform _leftMiddleFinger;
        [SerializeField] private Transform _leftRingFinger;
        [SerializeField] private Transform _leftPinkyFinger;

        [Header("Right (Top)")]
        [SerializeField]
        private Transform _rightShoulder;

        [SerializeField] private Transform _rightUpperArm;
        [SerializeField] private Transform _rightForeArm;
        [SerializeField] private Transform _rightWrist;
        [SerializeField] private Transform _rightThumbFinger;
        [SerializeField] private Transform _rightIndexFinger;
        [SerializeField] private Transform _rightMiddleFinger;
        [SerializeField] private Transform _rightRingFinger;
        [SerializeField] private Transform _rightPinkyFinger;

        protected override void Start()
        {
            _servoMixerService = Robot.Instance.GetServiceOfType<ServoMixerService>();
            base.Start();
        }

        public override void Initialize()
        {
            StartCoroutine(Loop());
        }

        public override void Dispose()
        {
            StopAllCoroutines();
        }

        private IEnumerator Loop()
        {
            Running = true;

            while (Running)
            {
                // Head
                var head = _head.rotation.eulerAngles;
                _servoMixerService.SetServoValueInEuler(ServoIdentifier.HeadYaw, head.y);

                // Neck
                var neck = _neck.rotation.eulerAngles;
                _servoMixerService.SetServoValueInEuler(ServoIdentifier.HeadPitch, neck.x);
                _servoMixerService.SetServoValueInEuler(ServoIdentifier.HeadRollPrimary, neck.z);

                // Pelvis
                var hips = _hip.rotation.eulerAngles;
                _servoMixerService.SetServoValueInEuler(ServoIdentifier.PelvisYaw, hips.y);
                _servoMixerService.SetServoValueInEuler(ServoIdentifier.PelvisPitch, hips.x);
                _servoMixerService.SetServoValueInEuler(ServoIdentifier.PelvisRollPrimary, hips.z);

                // Left Side
                var upperArm = _leftUpperArm.eulerAngles;
                var lowerArm = _leftForeArm.eulerAngles;
                var wrist = _leftWrist.eulerAngles;

                _servoMixerService.SetServoValueInEuler(ServoIdentifier.LeftShoulderYaw, upperArm.y);
                _servoMixerService.SetServoValueInEuler(ServoIdentifier.LeftShoulderPitch, upperArm.x);
                _servoMixerService.SetServoValueInEuler(ServoIdentifier.LeftShoulderRoll, upperArm.z);
                _servoMixerService.SetServoValueInEuler(ServoIdentifier.LeftArm, lowerArm.x);
                _servoMixerService.SetServoValueInEuler(ServoIdentifier.LeftWrist, wrist.z);

                // Left Hand
                var thumb = _leftThumbFinger.eulerAngles;
                var indexFinger = _leftIndexFinger.eulerAngles;
                var middleFinger = _leftMiddleFinger.eulerAngles;
                var ringFinger = _leftRingFinger.eulerAngles;
                var pinkyFinger = _leftPinkyFinger.eulerAngles;
                _servoMixerService.SetServoValueInEuler(ServoIdentifier.LeftFingerThumb, thumb.x);
                _servoMixerService.SetServoValueInEuler(ServoIdentifier.LeftFingerIndex, indexFinger.x);
                _servoMixerService.SetServoValueInEuler(ServoIdentifier.LeftFingerMiddle, middleFinger.x);
                _servoMixerService.SetServoValueInEuler(ServoIdentifier.LeftFingerRing, ringFinger.x);
                _servoMixerService.SetServoValueInEuler(ServoIdentifier.LeftFingerPinky, pinkyFinger.x);

                // Right Side
                upperArm = _rightUpperArm.eulerAngles;
                lowerArm = _rightForeArm.eulerAngles;
                wrist = _rightWrist.eulerAngles;

                _servoMixerService.SetServoValueInEuler(ServoIdentifier.RightShoulderYaw, upperArm.y);
                _servoMixerService.SetServoValueInEuler(ServoIdentifier.RightShoulderPitch, upperArm.x);
                _servoMixerService.SetServoValueInEuler(ServoIdentifier.RightShoulderRoll, upperArm.z);
                _servoMixerService.SetServoValueInEuler(ServoIdentifier.RightArm, lowerArm.x);
                _servoMixerService.SetServoValueInEuler(ServoIdentifier.RightWrist, wrist.z);

                // Right Hand
                thumb = _rightThumbFinger.eulerAngles;
                indexFinger = _rightIndexFinger.eulerAngles;
                middleFinger = _rightMiddleFinger.eulerAngles;
                ringFinger = _rightRingFinger.eulerAngles;
                pinkyFinger = _rightPinkyFinger.eulerAngles;
                _servoMixerService.SetServoValueInEuler(ServoIdentifier.RightFingerThumb, thumb.x);
                _servoMixerService.SetServoValueInEuler(ServoIdentifier.RightFingerIndex, indexFinger.x);
                _servoMixerService.SetServoValueInEuler(ServoIdentifier.RightFingerMiddle, middleFinger.x);
                _servoMixerService.SetServoValueInEuler(ServoIdentifier.RightFingerRing, ringFinger.x);
                _servoMixerService.SetServoValueInEuler(ServoIdentifier.RightFingerPinky, pinkyFinger.x);

                yield return CoroutineFactory.WaitForSeconds(_updateInterval);
            }
        }

        [ContextMenu("Populate Bones")]
        public void ParseRig()
        {
            if (_rig == null) return;
            ParseRig(_rig);
        }

        private void ParseRig(Transform rig)
        {
            var transforms = rig.GetComponentsInChildren<Transform>();

            foreach (var tr in transforms)
            {
                var trName = tr.name.ToLower();

                // Center
                if (Contains(trName, "hip"))
                    _hip = tr;
                else if (Contains(trName, "neck"))
                    _neck = tr;
                else if (Contains(trName, "head"))
                    _head = tr;

                // Left Arm
                else if (Contains(trName, "left", "shoulder"))
                    _leftShoulder = tr;
                else if (Contains(trName, "left", "upper", "arm"))
                    _leftUpperArm = tr;
                else if (Contains(trName, "left", "forearm"))
                    _leftForeArm = tr;
                else if (Contains(trName, "left", "wrist"))
                    _leftWrist = tr;

                // Left Hand
                else if (Contains(trName, "left", "thumb"))
                    _leftThumbFinger = tr;
                else if (Contains(trName, "left", "finger", "index"))
                    _leftIndexFinger = tr;
                else if (Contains(trName, "left", "finger", "middle"))
                    _leftMiddleFinger = tr;
                else if (Contains(trName, "left", "finger", "ring"))
                    _leftRingFinger = tr;
                else if (Contains(trName, "left", "finger", "pinky"))
                    _leftPinkyFinger = tr;

                // right Arm
                else if (Contains(trName, "right", "shoulder"))
                    _rightShoulder = tr;
                else if (Contains(trName, "right", "upper", "arm"))
                    _rightUpperArm = tr;
                else if (Contains(trName, "right", "forearm"))
                    _rightForeArm = tr;
                else if (Contains(trName, "right", "wrist"))
                    _rightWrist = tr;

                // right Hand
                else if (Contains(trName, "right", "thumb"))
                    _rightThumbFinger = tr;
                else if (Contains(trName, "right", "finger", "index"))
                    _rightIndexFinger = tr;
                else if (Contains(trName, "right", "finger", "middle"))
                    _rightMiddleFinger = tr;
                else if (Contains(trName, "right", "finger", "ring"))
                    _rightRingFinger = tr;
                else if (Contains(trName, "right", "finger", "pinky"))
                    _rightPinkyFinger = tr;
            }
        }

        private bool Contains(string target, params string[] search)
        {
            foreach (var item in search)
            {
                if (!target.Contains(item))
                    return false;
            }

            return true;
        }
    }
}