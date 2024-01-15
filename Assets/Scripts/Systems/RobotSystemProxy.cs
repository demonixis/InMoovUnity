using UnityEngine;

namespace Demonixis.InMoovUnity.Systems
{
    public abstract class RobotSystemProxy : MonoBehaviour
    {
        private void Start()
        {
            UnityRobot.Instance.OnRobotReady(Initialize);
        }

        protected abstract void Initialize(UnityRobot robot);

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
