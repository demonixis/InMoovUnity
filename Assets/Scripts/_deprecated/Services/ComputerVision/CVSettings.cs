using System;
using UnityEngine;

namespace Demonixis.InMoov.ComputerVision
{
    [Serializable]
    public struct CVSettings
    {
        public int WebCamLeft;
        public int WebCamRight;
        public int SaveFlag;

        public bool IsValid() => SaveFlag > 0;

        public bool HaveTwoCameras()
        {
            return WebCamLeft != WebCamRight;
        }

        public bool IsCameraConnected(bool left)
        {
            var devices = WebCamTexture.devices;

            if (left)
                return WebCamLeft < devices.Length;

            return WebCamRight < devices.Length;
        }
    }
}