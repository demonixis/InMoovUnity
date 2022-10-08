using System;
using Demonixis.InMoov.Servos;

namespace Demonixis.InMoov
{
    [Serializable]
    public sealed class ImSettings
    {
        public SerialData[] SerialData;
        public ServoData[] Servos;
    }
}