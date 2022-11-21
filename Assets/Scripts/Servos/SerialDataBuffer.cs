using System;
using System.IO.Ports;
using UnityEngine;

namespace Demonixis.InMoov.Servos
{
    [Serializable]
    public sealed class SerialDataBuffer
    {
        private readonly static int DataBufferSize = (SerialPortManager.PinEnd - SerialPortManager.PinStart) * 2;

        public byte[] DataBuffer { get; private set; }

        public SerialDataBuffer()
        {
            DataBuffer = new byte[DataBufferSize];
        }

        public void SetValue(int pinNumber, byte value, byte enabled)
        {
            if (pinNumber < SerialPortManager.PinStart || pinNumber > SerialPortManager.PinEnd)
            {
                Debug.LogError($"Pin {pinNumber} is not a valid pin number");
                return;
            }

            var index = (pinNumber - SerialPortManager.PinStart) * 2;
            DataBuffer[index] = value;
            DataBuffer[index + 1] = enabled;
        }

        public void Send(SerialPort serialPort)
        {
            serialPort.Write(DataBuffer, 0, DataBuffer.Length);
        }

        public void ClearData()
        {
            for (var i = 0; i < DataBuffer.Length; i++)
                DataBuffer[i] = 0;
        }

        public override string ToString()
        {
            return String.Join(":", DataBuffer);
        }
    }
}
