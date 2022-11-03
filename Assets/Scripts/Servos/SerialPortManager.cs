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

    public enum ArduinoIdentifiers
    {
        Left = 0,
        Right,
        Card3,
        Card4,
        Card5,
        Card6
    }

    [Serializable]
    public sealed class SerialPortManager : MonoBehaviour
    {
        public const int PinStart = 2;
        public const int PinEnd = 13;
        public const string SerialFilename = "serial.json";
        public const int DefaultBaudRate = 11500;
        private Dictionary<int, SerialPort> _serialPorts;
        
        /// <summary>
        /// An array that contains the card Id as key
        /// And contains an array of values
        /// Index 0 == PinStart
        /// Index Last = PinEnd
        /// </summary>
        private Dictionary<int, int[]> _dataValues;
        private bool _disposed;

        public static int[] DefaultPinValuesArray => new int[PinEnd - PinStart];
        
        public bool IsConnected(int cardId)
        {
            return _serialPorts.ContainsKey(cardId) && _serialPorts[cardId].IsOpen;
        }

        public void Initialize()
        {
            _dataValues = new Dictionary<int, int[]>();
            _serialPorts = new Dictionary<int, SerialPort>();
            
            var savedData =
                SaveGame.LoadRawData<SerialData[]>(SaveGame.GetPreferredStorageMode(), SerialFilename, "Config");

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
                SerialFilename, "Config");

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

        public void SendPinValues(int cardId)
        {
            if (_serialPorts == null) return;
            if (!_serialPorts.ContainsKey(cardId)) return;

            var values = _dataValues[cardId];
            for (var i = 0; i < values.Length; i++)
                _serialPorts[cardId].Write($"{values[i]}");
        }

        public void SetValueForCard(int cardId, int pinNumber, int value)
        {
            if (!_serialPorts.ContainsKey(cardId))
            {
                // TODO add a message
                return;
            }

            _dataValues[cardId][pinNumber] = value;
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
                    _dataValues.Add(cardId, DefaultPinValuesArray);
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