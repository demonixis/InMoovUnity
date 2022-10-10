using UnityEngine;

namespace Demonixis.InMoov
{
    public enum RobotServices
    {
        None,
        Voice,
        Ears,
        Chat,
        Vision,
        Servo,
        Other
    }

    public enum RobotPlatform
    {
        Windows,
        Linux,
        Mac,
        Android,
        All
    }

    public abstract class RobotService : MonoBehaviour
    {
        public virtual RobotPlatform[] SupportedPlateforms => new[] {RobotPlatform.All};

        public abstract RobotServices Type { get; }
        public abstract void Initialize();
        public abstract void SetPaused(bool paused);
        public abstract void Shutdown();
    }
}