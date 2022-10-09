using System.Collections;
using UnityEngine;

namespace Demonixis.InMoov.Servos
{
    public class ServoMixer : ImService
    {
        private ServoData[] _data;

        public float UpdateFrequency = 1.0f / 10.0f;

        public override ImServices Type => ImServices.Servo;

        public override void Initialize()
        {
        }

        public override void SetPaused(bool paused)
        {
        }

        public override void Shutdown()
        {
        }
    }
}