using System;
using System.Collections.Generic;
using System.IO.Ports;
using Demonixis.ToolboxV2;
using UnityEngine;

namespace Demonixis.InMoov.Servos
{
    [Serializable]
    public struct SerialData
    {
        public int CardId;
        public string PortName;
    }

    [Serializable]
    public sealed class SerialPortManager : MonoBehaviour
    {
        public const string SerialFilename = "serial.json";
        public const int DefaultBaudRate = 11500;
        private Dictionary<int, SerialPort> _serialPorts;
        private bool _disposed;

        public void Initialize()
        {
            _serialPorts = new Dictionary<int, SerialPort>();
            var savedData =
                SaveGame.LoadRawData<SerialData[]>(SaveGame.GetPreferredStorageMode(), SerialFilename);

            if (savedData is not {Length: > 0}) return;

            foreach (var data in savedData)
                Connect(data.CardId, data.PortName);
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
                SerialFilename);

            foreach (var serial in _serialPorts)
                serial.Value.Dispose();

            _disposed = true;
        }

        private void Shutdown()
        {
            Dispose();
        }

        public void SendBytes(int cardId, byte[] data)
        {
            if (_serialPorts == null) return;
            if (!_serialPorts.ContainsKey(cardId)) return;

            _serialPorts[cardId].Write(data, 0, data.Length);
        }

        public void SendPrefixedIntegers(int cardId, int[] values)
        {
            if (_serialPorts == null) return;
            if (!_serialPorts.ContainsKey(cardId)) return;

            for (var i = 0; i < values.Length; i++)
                _serialPorts[cardId].Write($"{i}_{values[i]}");
        }

        public void SendOrderedIntegers(int cardId, int[] values)
        {
            if (_serialPorts == null) return;
            if (!_serialPorts.ContainsKey(cardId)) return;

            for (var i = 0; i < values.Length; i++)
                _serialPorts[cardId].Write($"{values[i]}");
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
                
                if (serialPort != null)
                {
                    if (serialPort.IsOpen)
                        serialPort.Close();

                    serialPort.Dispose();
                }
            }

            return false;
        }
    }
}