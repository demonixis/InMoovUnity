using Demonixis.InMoov.Servos;
using Demonixis.InMoov.Utils;
using Demonixis.ToolboxV2.XR;
using System.Collections;
using UnityEngine;
using UnityEngine.SpatialTracking;

namespace Demonixis.InMoov
{
    public class XRRig : MonoBehaviour
    {
        private ServoMixerService _servoMixerService;
        private TrackedPoseDriver[] _trackedPoseDrivers;

        [SerializeField] private Camera _camera;
        [SerializeField] private AudioListener _audioListener;
        [SerializeField] private Transform[] _handOffsets;
        [SerializeField] private float _sendInterval = 1.0f / 60.0f;
        [SerializeField] private bool _startActive;
        [SerializeField] private bool _isRobot;

        public bool IsRobot => _isRobot;
        public bool IsActive => _camera.enabled;

        private void Awake()
        {
            _trackedPoseDrivers = GetComponentsInChildren<TrackedPoseDriver>();
        }

        private IEnumerator Start()
        {
            XRManager.SetTrackingOriginMode(UnityEngine.XR.TrackingOriginModeFlags.Device, true);
            SetActive(_startActive);

            if (XRManager.IsOpenXREnabled() && XRManager.Vendor == XRVendor.Oculus)
            {
                foreach (var offset in _handOffsets)
                    offset.localEulerAngles = new Vector3(60, 0, 0);
            }

            yield return null;

            _servoMixerService = Robot.Instance.ServoMixer;
        }

        private IEnumerator PawnControllerOverride()
        {
            while (true)
            {
                var camRot = _camera.transform.eulerAngles;

                _servoMixerService.SetServoValue(ServoIdentifier.HeadYaw, (byte)camRot.y);
                _servoMixerService.SetServoValue(ServoIdentifier.HeadPitch, (byte)camRot.x);
                _servoMixerService.SetServoValue(ServoIdentifier.HeadRoll, (byte)camRot.z);

                yield return CoroutineFactory.WaitForSeconds(_sendInterval);
            }
        }

        public void SetActive(bool active)
        {
            _camera.enabled = active;
            _audioListener.enabled = active;

            foreach (var trackedPoseDriver in _trackedPoseDrivers)
                trackedPoseDriver.enabled = active;

            if (_isRobot)
            {
                // TODO enable stereo
                // TODO Store preferred camera name for left and right eyes
                var webcamDisplay = GetComponentInChildren<WebCamDisplayer>();
                webcamDisplay.SetActive(active, 0);

                // TODO enable inverse kinematic
            }
        }
    }
}