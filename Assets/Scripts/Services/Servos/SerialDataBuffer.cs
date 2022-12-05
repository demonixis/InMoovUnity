using System;
using System.IO.Ports;
using UnityEngine;

namespace Demonixis.InMoov.Servos
{
    [Serializable]
    public sealed class SerialDataBuffer
    {
        public byte[] DataBuffer { get; private set; }

        public SerialDataBuffer()
        {
            DataBuffer = new byte[SerialPortManager.BufferLengthMega];
        }

        public void SetValue(int pinNumber, byte value, byte enabled)
        {
            if (pinNumber < SerialPortManager.PinStart || pinNumber > SerialPortManager.PinEndMega)
            {
                Debug.LogError($"Pin {pinNumber} is not a valid pin number");
                return;
            }

            var index = (pinNumber - SerialPortManager.PinStart) * 2;
            DataBuffer[index] = value;
            DataBuffer[index + 1] = enabled;
        }

        public void ClearData()
        {
            for (var i = 0; i < DataBuffer.Length; i++)
                DataBuffer[i] = 0;
        }

        public override string ToString()
        {
            return string.Join(":", DataBuffer);
        }
    }
}
