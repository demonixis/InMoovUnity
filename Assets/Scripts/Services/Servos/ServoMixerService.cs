using System;
using System.Collections;
using System.Collections.Generic;
using Demonixis.InMoov.Settings;
using Demonixis.InMoov.Utils;
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
        private List<ServoMixage> _servoMix;
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
                _servoData[i] = ServoData.New((ServoIdentifier)i);

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

            StartCoroutine(ServoLoop());
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

                    if (cardIndex >= 0)
                    {
                        if (data.MixageType != ServoMixageType.None)
                        {
                            if (data.MixageType == ServoMixageType.SameValue)
                                SetServoValueInServo(data.MixedServo, data.Value);
                            else if (data.MixageType == ServoMixageType.InverseValue)
                                SetServoValueInServo(data.MixedServo, (byte)(180 - data.Value));
                        }

                        _serialDataBuffer[cardIndex].SetValue(data.PinId, data.Value, data.Enabled);
                    }
                }

                // Send values.
                if (!_paused)
                {
                    for (var i = 0; i < _serialDataBuffer.Length; i++)
                        _serialPortManager.SendData(i, _serialDataBuffer[i]);
                }

                yield return CoroutineFactory.WaitForSeconds(_updateInterval);
            }
        }

        public int GetServoValue(ServoIdentifier servoId)
        {
            return _servoData[(int)servoId].Value;
        }

        public void SetServoValueInEuler(ServoIdentifier servoId, float rawValue)
        {
            ref var data = ref _servoData[(int)servoId];

            var value = ServoConverter.UnityRotationToServo(rawValue, data.ScaleValueTo180 > 0);
            
            // Apply servo data
            //value = (byte)Mathf.Max(data.Min, value);
            //value = (byte)Mathf.Min(data.Max, value);

            // Reverse
            if (data.Inverse)
                value = (byte)(180 - value);

            data.Value = value;
        }

        public void SetServoValueInServo(ServoIdentifier servoId, byte value)
        {
            ref var data = ref _servoData[(int)servoId];
            
            // Apply servo data
            //value = (byte)Mathf.Max(data.Min, value);
            //value = (byte)Mathf.Min(data.Max, value);

            // Reverse
            if (data.Inverse)
                value = (byte)(180 - value);
            
            data.Value = value;
        }

        public void SetServosToNeutral()
        {
            for (var i = 0; i < _servoData.Length; i++)
            {
                ref var data = ref _servoData[i];
                data.Value = data.Neutral;
            }
        }

        public void SetServoData(ServoIdentifier servoId, ref ServoData data)
        {
            var index = (int)servoId;
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
    }
}