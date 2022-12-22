using System;

namespace MSSpeechLink
{
    [Serializable]
    public enum MessageType
    {
        None = 0,
        VoiceRecognitionResult,
        Speak,
        SetLanguage,
        GetVoices,
        SetVoice,
        SetVoiceByIndex
    }

    [Serializable]
    public struct MessageData
    {
        public MessageType MessageType;
        public string Message;
    }
}