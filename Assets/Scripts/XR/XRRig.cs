using Demonixis.ToolboxV2.XR;
using UnityEngine;
using UnityEngine.SpatialTracking;

namespace Demonixis.InMoov
{
    public class XRRig : MonoBehaviour
    {
        private TrackedPoseDriver[] _trackedPoseDrivers;

        [SerializeField] private Camera _camera;
        [SerializeField] private AudioListener _audioListener;
        [SerializeField] private Transform[] _handOffsets;
        [SerializeField] private bool _startActive;

        private void Awake()
        {
            _trackedPoseDrivers = GetComponentsInChildren<TrackedPoseDriver>();
        }

        private void Start()
        {
            XRManager.SetTrackingOriginMode(UnityEngine.XR.TrackingOriginModeFlags.Device, true);
            SetActive(_startActive);
        
            if (XRManager.IsOpenXREnabled() && XRManager.Vendor == XRVendor.Oculus)
            {
                foreach (var offset in _handOffsets)
                    offset.localEulerAngles = new Vector3(60, 0, 0);
            }
        }

        public void SetActive(bool active)
        {
            _camera.enabled = active;
            _audioListener.enabled = active;

            foreach (var trackedPoseDriver in _trackedPoseDrivers)
                trackedPoseDriver.enabled = active;
        }
    }
}