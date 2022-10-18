using System;
using System.Collections;
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
        private int[] _servoStates;
        private List<int> _lockedServos;
        private bool _running;

        [SerializeField] private float _updateInterval = 1.0f / 30.0f;

        public override RobotServices Type => RobotServices.Servo;

        public override void Initialize()
        {
            base.Initialize();

            // Initialize data
            var names = Enum.GetNames(typeof(ServoIdentifier));
            var servoCount = names.Length;

            _servoData = new ServoData [servoCount];
            _servoValues = new int [servoCount];
            _servoStates = new int [servoCount];

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
            {
                _servoValues[i] = usePreviousValues ? servoMixerValues[i] : _servoData[i].Neutral;
                _servoStates[i] = _servoData[i].Enabled ? 1 : 0;
            }

            _lockedServos = new List<int>();
            _serialPortManager = FindObjectOfType<SerialPortManager>();
            _serialPortManager.Initialize();
        }

        public override void SetPaused(bool paused)
        {
            _running = !paused;

            if (_running)
                StartCoroutine(ServoLoop());
        }

        public override void Shutdown()
        {
            SaveGame.SaveRawData(SaveGame.GetPreferredStorageMode(), _servoData, ServoMixerFilename);
            SaveGame.SaveRawData(SaveGame.GetPreferredStorageMode(), _servoValues, ServoMixerValuesFilename);
            _serialPortManager.Dispose();
        }

        private IEnumerator ServoLoop()
        {
            _running = true;

            var wait = new WaitForSeconds(_updateInterval);

            while (_running)
            {
                // Send data to arduinos
                for (var i = 0; i < _servoData.Length; i++)
                {
                    var rotation = GetServoRotation(ref _servoData[i], _servoValues[i]);
                    _serialPortManager.SetValueForCard(_servoData[i].CardId, _servoData[i].PinId, rotation);
                }

                for (var i = 0; i < 4; i++)
                    _serialPortManager.SendPinValues(i);

                yield return wait;
            }
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
            var index = (int) servoId;
            _servoData[index] = data;
            _servoStates[index] = data.Enabled ? 1 : 0;
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
            return GetServoRotation(ref servo, rawValue);
        }

        private int GetServoRotation(ref ServoData servo, int rawValue)
        {
            rawValue = Mathf.Max(servo.Min, rawValue);
            rawValue = Mathf.Min(servo.Max, rawValue);

            if (servo.Inverse)
                rawValue *= -1;

            return rawValue;
        }
    }
}