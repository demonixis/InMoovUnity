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
        private SerialDataBuffer[] _serialDataBuffer;
        private List<int> _lockedServos;
        private bool _running;
        private bool _paused;

        [SerializeField] private float _updateInterval = 1.0f / 30.0f;

        public override RobotServices Type => RobotServices.Servo;

        public override void Initialize()
        {
            base.Initialize();

            // Initialize data
            var names = Enum.GetNames(typeof(ServoIdentifier));
            var servoCount = names.Length;

            _servoData = new ServoData[servoCount];
            for (var i = 0; i < servoCount; i++)
                _servoData[i] = ServoData.New(names[i]);

            // Load saved data and apply them
            var servoMixerData =
                SaveGame.LoadRawData<ServoData[]>(SaveGame.GetPreferredStorageMode(), ServoMixerFilename, "Config");

            if (servoMixerData != null && servoMixerData.Length == servoCount)
                _servoData = servoMixerData;

            names = Enum.GetNames(typeof(ArduinoIdentifiers));
            _serialDataBuffer = new SerialDataBuffer[names.Length];
            for (var i = 0; i < _serialDataBuffer.Length; i++)
                _serialDataBuffer[i] = new SerialDataBuffer();

            _lockedServos = new List<int>();
            _serialPortManager = GetComponent<SerialPortManager>();
            _serialPortManager.Initialize();
        }

        public override void SetPaused(bool paused)
        {
            _paused = !paused;
        }

        public override void Shutdown()
        {
            SaveGame.SaveRawData(SaveGame.GetPreferredStorageMode(), _servoData, ServoMixerFilename, "Config");
            _serialPortManager.Dispose();
        }

        private IEnumerator ServoLoop()
        {
            _running = true;

            var wait = new WaitForSeconds(_updateInterval);

            while (_running)
            {
                // Clear previous values.
                for (var i = 0; i < _serialDataBuffer.Length; i++)
                    _serialDataBuffer[i].ClearData();

                // Map new values.
                for (var i = 0; i < _servoData.Length; i++)
                {
                    var data = _servoData[i];
                    var cardIndex = (int)data.CardId;
                    var value = GetServoValue((ServoIdentifier)i);
                    _serialDataBuffer[cardIndex].SetValue(data.PinId, data.Value, data.Enabled);
                }

                // Send values.
                if (!_paused)
                {
                    for (var i = 0; i < _serialDataBuffer.Length; i++)
                        _serialPortManager.SendData(i, _serialDataBuffer[i]);
                }

                yield return wait;
            }
        }

        public int GetServoValue(ServoIdentifier servoId)
        {
            return _servoData[(int)servoId].Value;
        }

        public void SetServoValue(ServoIdentifier servoId, byte value)
        {
            ref var data = ref _servoData[(int)servoId];
            data.Value = value;
        }

        public void SetServoData(ServoIdentifier servoId, ServoData data)
        {
            var index = (int)servoId;
            var previousData = _servoData[index];
            data.Value = previousData.Value;
            _servoData[index] = data;
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

            // TODO Inverse

            return rawValue;
        }
    }
}