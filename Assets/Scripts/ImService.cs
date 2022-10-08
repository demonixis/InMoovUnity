using UnityEngine;

namespace Demonixis.InMoov
{
    public enum ImServices
    {
        None, Voice, Ears, Chat, Vision, Servo
    }
    
    public abstract class ImService
    {
        public abstract ImServices Type { get; }
        public abstract void Initialize();
        public abstract void SetPaused(bool paused);
        public abstract void Shutdown();
    }
}