using System;
using System.Collections.Generic;
using System.IO.Ports;
using Demonixis.InMoov.Settings;
using UnityEngine;

namespace Demonixis.InMoov.Servos
{
    [Serializable]
    public struct SerialData
    {
        public int CardId;
        public string PortName;
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

    [Serializable]
    public sealed class SerialPortManager : MonoBehaviour
    {
        public const int PinStart = 2;
        public const int PinEnd = 13;
        public const string SerialFilename = "serial.json";
        public const int DefaultBaudRate = 11500;
        private Dictionary<int, SerialPort> _serialPorts;
        private bool _disposed;

#if UNITY_EDITOR
        [SerializeField] private bool _logFirstTrame = true;
#endif

        public bool IsConnected(int cardId)
        {
            return _serialPorts.ContainsKey(cardId) && _serialPorts[cardId].IsOpen;
        }

        public void Initialize()
        {
            _serialPorts = new Dictionary<int, SerialPort>();

            var savedData =
                SaveGame.LoadRawData<SerialData[]>(SaveGame.GetPreferredStorageMode(), SerialFilename, "Config");

            if (savedData != null && savedData.Length > 0)
            {
                foreach (var data in savedData)
                    Connect(data.CardId, data.PortName);
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
            if (serialPort == null) return;

            buffer.Send(serialPort);
        }

        public bool Connect(int cardId, string serialName)
        {
            if (_serialPorts.ContainsKey(cardId)) return false;

            SerialPort serialPort = null;

            try
            {
                serialPort = new SerialPort(serialName, DefaultBaudRate);
                serialPort.Open();

                if (serialPort.IsOpen)
                {
                    _serialPorts.Add(cardId, serialPort);
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