using UnityEngine;

namespace Demonixis.InMoov
{
    public enum RobotServices
    {
        None,
        Voice,
        Ears,
        Chat,
        Animation,
        ComputerVision,
        Navigation,
        Servo,
        Other
    }
    
    /// <summary>
    /// Base skeleton of a robot service.
    /// A service must have a type. By default it is supported on all platforms.
    /// </summary>
    public abstract class RobotService : MonoBehaviour
    {
        public virtual RuntimePlatform[] SupportedPlateforms => new[]
        {
            RuntimePlatform.Android,
            RuntimePlatform.LinuxEditor,
            RuntimePlatform.LinuxPlayer,
            RuntimePlatform.WindowsEditor,
            RuntimePlatform.WindowsPlayer,
            RuntimePlatform.OSXEditor,
            RuntimePlatform.OSXPlayer
        };

        public string ServiceName => GetType().Name;
        public abstract RobotServices Type { get; }
        public bool Initialized { get; protected set; }

        public virtual void Initialize()
        {
            Initialized = true;
        }
        
        public abstract void SetPaused(bool paused);

        public virtual void Shutdown()
        {
            Initialized = false;
        }

        public bool IsSupported()
        {
            var plateform = Application.platform;
            
            foreach (var platform in SupportedPlateforms)
            {
                if (platform == plateform) return true;
            }

            return false;
        }
    }
}