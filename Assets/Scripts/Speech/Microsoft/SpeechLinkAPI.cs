using System;

namespace MSSpeechLink
{
    [Serializable]
    public enum MessageType
    {
        None = 0,
        VoiceRecognized,
        TextToSpeech,
        SelectLang,
        ListVoices,
        SelectVoice
    }

    [Serializable]
    public struct MessageData
    {
        public MessageType MessageType;
        public string Message;
    }
}
