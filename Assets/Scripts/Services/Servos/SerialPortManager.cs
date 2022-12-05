using Demonixis.InMoov.Settings;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;

namespace Demonixis.InMoov.Servos
{
    [Serializable]
    public struct SerialData
    {
        public int CardId;
        public string PortName;
        public bool IsMega2560;
    }

    public enum ArduinoIdentifiers
    {
        Left = 0,
        Right,
        Card3,
        Card4,
        Card5,
        Card6,
        None
    }

    public struct SerialPortProxy
    {
        public SerialPort Serial;
        public bool IsMega2560;

        public bool IsOpen => Serial?.IsOpen ?? false;
        public string PortName => Serial?.PortName ?? string.Empty;

        public void Dispose()
        {
            if (Serial == null) return;
            Serial.Dispose();
        }
    }

    [Serializable]
    public sealed class SerialPortManager : MonoBehaviour
    {
        public const int PinStart = 2;
        public const int PinEnd = 13;
        public const int PinEndMega = 53;
        public const string SerialFilename = "serial.json";
        public const int DefaultBaudRate = 9600;
        private Dictionary<int, SerialPortProxy> _serialPorts;
        private bool _disposed;

#if UNITY_EDITOR
        [SerializeField] private bool _logFirstTrame = true;
        [SerializeField] private bool _logArduino = true;
#endif

        public static int MaximumServoCount => PinEnd - PinStart;
        public static int MaximumServoCountMega => PinEndMega - PinStart;

        public bool IsConnected(int cardId)
        {
            return _serialPorts.ContainsKey(cardId) && _serialPorts[cardId].IsOpen;
        }

        public bool IsMega2560(int cardId)
        {
            return _serialPorts.ContainsKey(cardId) && _serialPorts[cardId].IsMega2560;
        }

        public void Initialize()
        {
            _serialPorts = new Dictionary<int, SerialPortProxy>();

            var savedData =
                SaveGame.LoadRawData<SerialData[]>(SaveGame.GetPreferredStorageMode(), SerialFilename, "Config");

            if (savedData == null || savedData.Length <= 0) return;
            foreach (var data in savedData)
                Connect(data.CardId, data.PortName, data.IsMega2560);
        }

        public void Dispose()
        {
            if (_disposed) return;

            var serialData = new SerialData[_serialPorts.Count];
            var i = 0;
            foreach (var keyValue in _serialPorts)
            {
                serialData[i++] = new SerialData
                {
                    CardId = keyValue.Key,
                    PortName = keyValue.Value.PortName,
                    IsMega2560 = keyValue.Value.IsMega2560
                };
            }

            SaveGame.SaveRawData(SaveGame.GetPreferredStorageMode(), serialData,
                SerialFilename, "Config");

            foreach (var serial in _serialPorts)
                serial.Value.Dispose();

            _disposed = true;
        }

        public void SendData(int cardId, SerialDataBuffer buffer)
        {
#if UNITY_EDITOR
            if (_logFirstTrame && cardId == 0)
                Debug.Log($"{cardId}_{buffer}");
#endif

            if (_serialPorts == null || !_serialPorts.ContainsKey(cardId)) return;

            var serialPort = _serialPorts[cardId];
            if (serialPort.Serial == null) return;

            try
            {
                var bufferSize = serialPort.IsMega2560 ? MaximumServoCountMega : MaximumServoCount;
                serialPort.Serial.Write(buffer.DataBuffer, 0, bufferSize);
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }

        private void Update()
        {
            if (_serialPorts == null) return;

            foreach (var sp in _serialPorts)
            {
                if (sp.Value.Serial == null) continue;

                var result = sp.Value.Serial.ReadExisting();
#if UNITY_EDITOR
                if (_logArduino && !string.IsNullOrEmpty(result))
                    Debug.Log(result);
#endif
            }
        }

        public bool Connect(int cardId, string serialName, bool isMega2560)
        {
            if (_serialPorts.ContainsKey(cardId)) return false;

            SerialPort serialPort = null;

            try
            {
                serialPort = new SerialPort(serialName, DefaultBaudRate);
                serialPort.Open();

                if (serialPort.IsOpen)
                {
                    serialPort.ErrorReceived += (sender, e) => Debug.Log($"Error {e}");
                    serialPort.DataReceived += (sender, e) => Debug.Log($"Data Received: {e}");
                    _serialPorts.Add(cardId, new SerialPortProxy
                    {
                        Serial = serialPort,
                        IsMega2560 = isMega2560
                    });
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
            }

            if (serialPort != null)
            {
                if (serialPort.IsOpen)
                    serialPort.Close();

                serialPort.Dispose();
            }

            return false;
        }

        public void Disconnect(int cardId)
        {
            if (!_serialPorts.ContainsKey(cardId)) return;

            _serialPorts[cardId].Dispose();
            _serialPorts.Remove(cardId);
        }
    }
}