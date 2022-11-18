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
        private SerialDataBuffer[] _serialDataBuffers;
        private byte[] _servoValues;
        private byte[] _servoStates;
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

            _servoData = new ServoData[servoCount];
            _servoValues = new byte[servoCount];
            _servoStates = new byte[servoCount];

            for (var i = 0; i < servoCount; i++)
            {
                _servoData[i] = ServoData.New(names[i]);
                _servoValues[i] = _servoData[i].Neutral;
            }

            // Load saved data and apply them
            var servoMixerData =
                SaveGame.LoadRawData<ServoData[]>(SaveGame.GetPreferredStorageMode(), ServoMixerFilename, "Config");

            var servoMixerValues =
                SaveGame.LoadRawData<byte[]>(SaveGame.GetPreferredStorageMode(), ServoMixerValuesFilename, "Config");

            if (servoMixerData != null && servoMixerData.Length == servoCount)
                _servoData = servoMixerData;

            var usePreviousValues = servoMixerValues != null && servoMixerValues.Length == servoCount;

            for (var i = 0; i < servoCount; i++)
            {
                _servoValues[i] = usePreviousValues ? servoMixerValues[i] : _servoData[i].Neutral;
                _servoStates[i] = _servoData[i].Enabled;
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
            SaveGame.SaveRawData(SaveGame.GetPreferredStorageMode(), _servoData, ServoMixerFilename, "Config");
            SaveGame.SaveRawData(SaveGame.GetPreferredStorageMode(), _servoValues, ServoMixerValuesFilename, "Config");
            _serialPortManager.Dispose();
        }

        private IEnumerator ServoLoop()
        {
            _running = true;

            var wait = new WaitForSeconds(_updateInterval);

            while (_running)
            {   
                // TODO
                yield return wait;
            }
        }

        public int GetServoValue(ServoIdentifier servoId)
        {
            return _servoValues[(int)servoId];
        }

        public void SetServoValue(ServoIdentifier servoId, byte value)
        {
            _servoValues[(int)servoId] = value;
        }

        public void SetServoData(ServoIdentifier servoId, ServoData data)
        {
            var index = (int)servoId;
            _servoData[index] = data;
            _servoStates[index] = data.Enabled;
        }

        public ServoData GetServoData(ServoIdentifier id)
        {
            return _servoData[(int)id];
        }

        public void LockServo(ServoIdentifier id)
        {
            var index = (int)id;
            if (!_lockedServos.Contains(index))
                _lockedServos.Add(index);
        }

        public void UnlockServo(ServoIdentifier id)
        {
            var index = (int)id;
            if (_lockedServos.Contains(index))
                _lockedServos.Remove(index);
        }

        private int GetServoRotation(ServoIdentifier servoId, byte rawValue)
        {
            var servo = _servoData[(int)servoId];
            return GetServoRotation(ref servo, rawValue);
        }

        private byte GetServoRotation(ref ServoData servo, byte rawValue)
        {
            rawValue = (byte)Mathf.Max(servo.Min, rawValue);
            rawValue = (byte)Mathf.Min(servo.Max, rawValue);



            return rawValue;
        }
    }
}