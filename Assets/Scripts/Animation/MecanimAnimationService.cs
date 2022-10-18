using System.Collections;
using UnityEngine;

namespace Demonixis.InMoov.Animations
{
    public class MecanimAnimationService : AnimationService
    {
        private bool _running;

        [Header("Config")] [SerializeField] private float _updateInterval = 1.0f / 30.0f;

        [Header("Rig")] [SerializeField] private Transform _rig;
        [SerializeField] private Transform _head;
        [SerializeField] private Transform _neck;
        [SerializeField] private Transform _hip;

        [Header("Left (Top)")] [SerializeField]
        private Transform _leftShoulder;

        [SerializeField] private Transform _leftUpperArm;
        [SerializeField] private Transform _leftForeArm;
        [SerializeField] private Transform _leftWrist;
        [SerializeField] private Transform _leftThumbFinger;
        [SerializeField] private Transform _leftIndexFinger;
        [SerializeField] private Transform _leftMiddleFinger;
        [SerializeField] private Transform _leftRingFinger;
        [SerializeField] private Transform _leftPinkyFinger;

        [Header("Right (Top)")] [SerializeField]
        private Transform _rightShoulder;

        [SerializeField] private Transform _rightUpperArm;
        [SerializeField] private Transform _rightForeArm;
        [SerializeField] private Transform _rightWrist;
        [SerializeField] private Transform _rightThumbFinger;
        [SerializeField] private Transform _rightIndexFinger;
        [SerializeField] private Transform _rightMiddleFinger;
        [SerializeField] private Transform _rightRingFinger;
        [SerializeField] private Transform _rightPinkyFinger;

        public override RobotServices Type => RobotServices.Other;

        public override void Initialize()
        {
            StartCoroutine(Loop());
            base.Initialize();
        }

        private IEnumerator Loop()
        {
            _running = true;

            var interval = new WaitForSeconds(_updateInterval);

            while (_running)
            {
                var neckRotation = ClampedVector0to180(_neck.rotation.eulerAngles);


                yield return interval;
            }
        }

        private static Vector3 ClampedVector0to180(Vector3 target)
        {
            target.x = (target.x % 360.0f) / 2.0f;
            target.y = (target.y % 360.0f) / 2.0f;
            target.z = (target.z % 360.0f) / 2.0f;
            return target;
        }

        public override void SetPaused(bool paused)
        {
            _running = !paused;

            if (!paused)
                StartCoroutine(Loop());
        }

        public override void Shutdown()
        {
            _running = false;
            base.Shutdown();
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