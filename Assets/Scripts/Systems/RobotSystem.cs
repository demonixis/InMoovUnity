using System;
using UnityEngine;

namespace Demonixis.InMoov
{
    public abstract class RobotSystem : MonoBehaviour, IDisposable
    {
        [SerializeField] private bool _autoStart;

        public bool Running { get; protected set; }

        protected virtual void Start()
        {
            if (_autoStart)
                Initialize();
        }

        public abstract void Initialize();
        public abstract void Dispose();
    }
}