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
        public bool Mega2560;
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
        public bool Mega2560;

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
        public const int PinEndNonMega = 13;
        public const int PinEnd = 53;
        public const string SerialFilename = "serial.json";
        public const int DefaultBaudRate = 115200;
        private Dictionary<int, SerialPortProxy> _serialPorts;
        private bool _disposed;

#if UNITY_EDITOR
        [SerializeField] private bool _logFirstTrame = true;
#endif

        public static int BufferLengthNonMega => PinEndNonMega - PinStart;
        public static int BufferLength => PinEnd - PinStart;

        public bool IsConnected(int cardId)
        {
            return _serialPorts.ContainsKey(cardId) && _serialPorts[cardId].IsOpen;
        }

        public void Initialize()
        {
            _serialPorts = new Dictionary<int, SerialPortProxy>();

            var savedData =
                SaveGame.LoadRawData<SerialData[]>(SaveGame.GetPreferredStorageMode(), SerialFilename, "Config");

            if (savedData != null && savedData.Length > 0)
            {
                foreach (var data in savedData)
                    Connect(data.CardId, data.PortName, data.Mega2560);
            }
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
                    PortName = keyValue.Value.PortName
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
                serialPort.Serial.Write(buffer.DataBuffer, 0, serialPort.Mega2560 ? SerialPortManager.BufferLength : SerialPortManager.BufferLengthNonMega);
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
                Debug.Log(sp.Value.Serial.ReadExisting());
            }
        }

        public bool Connect(int cardId, string serialName, bool mega2560)
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
                        Mega2560 = mega2560
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