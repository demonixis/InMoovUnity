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
        public const string ServoMixerValuesFilename = "servos-values.json";
        private SerialPortManager _serialPortManager;
        private ServoData[] _servoData;
        private int[] _servoValues;
        private List<int> _lockedServos;

        public override RobotServices Type => RobotServices.Servo;

        public override void Initialize()
        {
            base.Initialize();
            
            // Initialize data
            var names = Enum.GetNames(typeof(ServoIdentifier));
            var servoCount = names.Length;

            _servoData = new ServoData [servoCount];
            _servoValues = new int [servoCount];

            for (var i = 0; i < servoCount; i++)
            {
                _servoData[i] = ServoData.New(names[i]);
                _servoValues[i] = _servoData[i].Neutral;
            }

            // Load saved data and apply them
            var servoMixerData =
                SaveGame.LoadRawData<ServoData[]>(SaveGame.GetPreferredStorageMode(), ServoMixerFilename);

            var servoMixerValues =
                SaveGame.LoadRawData<int[]>(SaveGame.GetPreferredStorageMode(), ServoMixerValuesFilename);

            if (servoMixerData != null && servoMixerData.Length == servoCount)
                _servoData = servoMixerData;

            var usePreviousValues = servoMixerValues != null && servoMixerValues.Length == servoCount;

            for (var i = 0; i < servoCount; i++)
                _servoValues[i] = usePreviousValues ? servoMixerValues[i] : _servoData[i].Neutral;

            _lockedServos = new List<int>();
            _serialPortManager = FindObjectOfType<SerialPortManager>();
            _serialPortManager.Initialize();
        }

        public override void SetPaused(bool paused)
        {
        }

        public override void Shutdown()
        {
            SaveGame.SaveRawData(SaveGame.GetPreferredStorageMode(), _servoData, ServoMixerFilename);
            SaveGame.SaveRawData(SaveGame.GetPreferredStorageMode(), _servoValues, ServoMixerValuesFilename);
            _serialPortManager.Dispose();
        }
        
        public int GetServoValue(ServoIdentifier servoId)
        {
            return _servoValues[(int) servoId];
        }

        public void SetServoValue(ServoIdentifier servoId, int value)
        {
            _servoValues[(int) servoId] = value;
        }

        public void SetServoData(ServoIdentifier servoId, ServoData data)
        {
            _servoData[(int) servoId] = data;
        }

        public ServoData GetServoData(ServoIdentifier id)
        {
            return _servoData[(int) id];
        }

        public void LockServo(ServoIdentifier id)
        {
            var index = (int) id;
            if (!_lockedServos.Contains(index))
                _lockedServos.Add(index);
        }

        public void UnlockServo(ServoIdentifier id)
        {
            var index = (int) id;
            if (_lockedServos.Contains(index))
                _lockedServos.Remove(index);
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