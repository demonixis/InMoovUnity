using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Net;
using System.Threading;

namespace Demonixis.InMoov
{
    [Serializable]
    public struct SerialData
    {
        public int CardId;
        public string PortName;
    }

    [Serializable]
    public sealed class ImSerial
    {
        public const int DefaultBaudRate = 11500;
        private Dictionary<int, SerialPort> _serialPorts;

        public ImSerial()
        {
            _serialPorts = new Dictionary<int, SerialPort>();
        }

        public void TryReconnect(IEnumerable<SerialData> data)
        {
            foreach (var item in data)
                Connect(item.CardId, item.PortName);
        }

        public void Connect(int cardId, string serialName)
        {
            if (_serialPorts.ContainsKey(cardId)) return;

            if (TryConnect(serialName, out SerialPort serialPort))
            {
                _serialPorts.Add(cardId, serialPort);
            }
        }

        public void SendData(int cardId, byte[] data)
        {
            if (_serialPorts == null) return;
            if (!_serialPorts.ContainsKey(cardId)) return;
            _serialPorts[cardId].Write(data, 0, data.Length);
        }

        private static bool TryConnect(string serialName, out SerialPort serialPort)
        {
            serialPort = null;

            try
            {
                serialPort = new SerialPort(serialName, DefaultBaudRate);
                serialPort.Open();
                return serialPort.IsOpen;
            }
            catch (Exception)
            {
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