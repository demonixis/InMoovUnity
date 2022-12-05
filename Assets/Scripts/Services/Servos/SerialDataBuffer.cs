using System;
using UnityEngine;

namespace Demonixis.InMoov.Servos
{
    [Serializable]
    public sealed class SerialDataBuffer
    {
        public const int ArduinoMaxBufferSize = 64;
        public byte[] DataBuffer { get; private set; }

        public SerialDataBuffer()
        {
            DataBuffer = new byte[ArduinoMaxBufferSize];
        }

        public void SetValue(int pinNumber, byte value, bool enabled)
        {
            if (pinNumber < SerialPortManager.PinStart || pinNumber > SerialPortManager.PinEndMega)
            {
                Debug.LogError($"Pin {pinNumber} is not a valid pin number");
                return;
            }

            var index = pinNumber - SerialPortManager.PinStart;
            DataBuffer[index] = enabled ? value : byte.MaxValue;
        }

        public void ClearData()
        {
            for (var i = 0; i < DataBuffer.Length; i++)
                DataBuffer[i] = byte.MaxValue;
        }

        public override string ToString()
        {
            return string.Join(":", DataBuffer);
        }
    }
}
