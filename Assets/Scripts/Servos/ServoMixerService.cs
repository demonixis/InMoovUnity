using System.Collections;
using UnityEngine;

namespace Demonixis.InMoov.Servos
{
    [RequireComponent(typeof(SerialPortManager))]
    public class ServoMixerService : RobotService
    {
        private ServoData[] _servoData;
        private int[] _servoValues;

        public float UpdateFrequency = 1.0f / 10.0f;

        public override RobotServices Type => RobotServices.Servo;

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