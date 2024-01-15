using System;
using UnityEngine;

namespace Demonixis.InMoov
{
    public abstract class RobotSystem : MonoBehaviour, IDisposable
    {
        public bool Started { get; protected set; }

        public virtual void Initialize()
        {
            Started = true;
        }

        public virtual void Dispose()
        {
            Started = false;
        }

        public void SetActive(bool active)
        {
            Robot.Instance.WhenStarted(() =>
            {
                InternalSetActive(active);
            });
        }

        private void InternalSetActive(bool active)
        {
            switch (active)
            {
                case true when !Started:
                    Initialize();
                    break;
                case false when Started:
                    Dispose();
                    break;
            }

            Started = active;
        }
    }
}