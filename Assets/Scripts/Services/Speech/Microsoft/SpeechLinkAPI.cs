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
        SelectVoice,
        SelectVoiceInt,
    }

    [Serializable]
    public struct MessageData
    {
        public MessageType MessageType;
        public string Message;
    }
}
