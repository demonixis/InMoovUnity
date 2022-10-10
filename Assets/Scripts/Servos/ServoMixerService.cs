using System;
using System.Collections.Generic;
using Demonixis.ToolboxV2;
using UnityEngine;

namespace Demonixis.InMoov.Servos
{
    [RequireComponent(typeof(SerialPortManager))]
    public class ServoMixerService : RobotService
    {
        public const string ServoMixerFilename = "servos.json";
        private SerialPortManager _serialPortManager;
        private ServoData[] _servoData;
        private Dictionary<int, int[]> _servoValues;

        public override RobotServices Type => RobotServices.Servo;

        public override void Initialize()
        {
            _servoData =
                SaveGame.LoadRawData<ServoData[]>(SaveGame.GetPreferredStorageMode(), ServoMixerFilename);

            if (_servoData == null || _servoData.Length == 0)
            {
                var names = Enum.GetNames(typeof(ServoIdentifier));
                _servoData = new ServoData [names.Length];
            }

            _serialPortManager.Initialize();
        }

        public override void SetPaused(bool paused)
        {
        }

        public override void Shutdown()
        {
            SaveGame.SaveRawData(SaveGame.GetPreferredStorageMode(), _servoData, ServoMixerFilename);
            _serialPortManager.Dispose();
        }

        public void ConnectArduino(int cardId, string serialName)
        {
            _serialPortManager.Connect(cardId, serialName);
        }

        public void SetServoData(ServoIdentifier servoId, ServoData data)
        {
            _servoData[(int) servoId] = data;
        }

        private int GetServoRotation(ServoIdentifier servoId, int rawValue)
        {
            var servo = _servoData[(int) servoId];
            
            rawValue = Mathf.Max(servo.Min, rawValue);
            rawValue = Mathf.Min(servo.Max, rawValue);

            if (servo.Inverse)
                rawValue *= -1;

            return rawValue;
        }
    }
}