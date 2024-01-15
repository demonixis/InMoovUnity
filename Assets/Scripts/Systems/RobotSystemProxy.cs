using UnityEngine;

namespace Demonixis.InMoovUnity.Systems
{
    public abstract class RobotSystemProxy : MonoBehaviour
    {
        private void Start()
        {
            UnityRobotProxy.Instance.OnRobotReady(Initialize);
        }

        protected abstract void Initialize(UnityRobotProxy robot);

        protected abstract void UpdateValues();

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (enabled)
                UpdateValues();
        }
#endif
    }
}
