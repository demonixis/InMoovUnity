﻿using UnityEngine;

namespace Demonixis.InMoov.Servos
{
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
    }
}