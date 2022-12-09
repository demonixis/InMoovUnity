using System;
using UnityEngine;

namespace Demonixis.InMoov
{
    public abstract class RobotSystem : MonoBehaviour, IDisposable
    {
        public bool Started { get; protected set; }

        public abstract void Initialize();
        public abstract void Dispose();
        
        public virtual void SetActive(bool active)
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