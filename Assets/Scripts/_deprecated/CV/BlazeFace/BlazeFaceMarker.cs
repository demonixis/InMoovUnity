using MediaPipe.BlazeFace;
using UnityEngine;

namespace Demonixis.InMoov.ComputerVision.Filters
{
    public sealed class BlazeFaceMarker : MonoBehaviour
    {
        public FaceDetector.Detection Detection { get; set; }
    }
}