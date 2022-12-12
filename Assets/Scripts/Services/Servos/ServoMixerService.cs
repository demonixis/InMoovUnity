using Demonixis.InMoov.Settings;
using Demonixis.InMoov.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Demonixis.InMoov.Servos
{
    [RequireComponent(typeof(SerialPortManager))]
    public class ServoMixerService : RobotService
    {
        public const string ServoMixerFilename = "servos.json";

        private SerialPortManager _serialPortManager;
        private ServoData[] _servoData;
        private SerialDataBuffer[] _serialDataBuffer;
        private bool _running;
        private bool _paused;

        [SerializeField] private float _updateInterval = 1.0f / 30.0f;

        public override void Initialize()
        {
            base.Initialize();

            // Initialize data
            var names = Enum.GetNames(typeof(ServoIdentifier));
            var servoCount = names.Length;

            _servoData = new ServoData[servoCount];
            for (var i = 0; i < servoCount; i++)
                _servoData[i] = ServoData.New((ServoIdentifier) i);

            // Load saved data and apply them
            var servoMixerData =
                SaveGame.LoadRawData<ServoData[]>(SaveGame.GetPreferredStorageMode(), ServoMixerFilename, "Config");

            if (servoMixerData != null && servoMixerData.Length == servoCount)
                _servoData = servoMixerData;

            names = Enum.GetNames(typeof(ArduinoIdentifiers));
            _serialDataBuffer = new SerialDataBuffer[names.Length];
            for (var i = 0; i < _serialDataBuffer.Length; i++)
                _serialDataBuffer[i] = new SerialDataBuffer();
            
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
                    var cardIndex = (int) data.CardId;

                    if (cardIndex < 0) continue;

                    // TODO add neutral offset
                    //var offset = 90 - data.Neutral;
                    //data.Value = (byte)(data.Value + offset);

                    ClampServoValue(data.Min, data.Max, ref data.Value);

                    if (data.Inverse)
                        InvertServoValue(ref data.Value);

                    if (data.Enabled && data.MixageType != ServoMixageType.None)
                    {
                        switch (data.MixageType)
                        {
                            case ServoMixageType.SameValue:
                                SetRawServoValue(data.MixedServo, data.Value);
                                break;
                            case ServoMixageType.InverseValue:
                                SetRawServoValue(data.MixedServo, (byte) (180 - data.Value));
                                break;
                        }
                    }

                    _serialDataBuffer[cardIndex].SetValue(data.PinId, data.Value, data.Enabled);
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
            return _servoData[(int) servoId].Value;
        }

        public void SetServoValueInEuler(ServoIdentifier servoId, float rawValue)
        {
            ref var data = ref _servoData[(int) servoId];
            var value = ServoConverter.UnityRotationToServo(rawValue, data.ScaleValueTo180 > 0);

            data.Value = value;
        }

        public void SetServoValueInServo(ServoIdentifier servoId, byte value)
        {
            ref var data = ref _servoData[(int) servoId];
            data.Value = value;
        }

        public void SetRawServoValue(ServoIdentifier servoId, byte value)
        {
            ref var data = ref _servoData[(int) servoId];
            data.Value = value;
        }

        private void ClampServoValue(byte min, byte max, ref byte value)
        {
            value = (byte) Mathf.Max(min, value);
            value = (byte) Mathf.Min(max, value);
        }

        private void InvertServoValue(ref byte value)
        {
            var inverted = 180 - value;
            value = (byte) inverted;
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
            var index = (int) servoId;
            _servoData[index] = data;
        }

        public ServoData GetServoData(ServoIdentifier id)
        {
            return _servoData[(int) id];
        }
    }
}