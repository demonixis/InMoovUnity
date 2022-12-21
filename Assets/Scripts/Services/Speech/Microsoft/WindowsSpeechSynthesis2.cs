#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
#define MS_SPEECH_SYNTHESIS
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

#if MS_SPEECH_SYNTHESIS
using System.Runtime.InteropServices;
#endif

namespace Demonixis.InMoov.Services.Speech
{
    [RequireComponent(typeof(AudioSource))]
    public class WindowsSpeechSynthesis2 : SpeechSynthesisService
    {
        const int TRUE = 1, FALSE = 0;

        [DllImport("TTSDLL", EntryPoint = "CreateTTS")]
        static extern IntPtr CreateTTS();

        [DllImport("TTSDLL", EntryPoint = "GetVoiceCount")]
        static extern int GetVoiceCount(IntPtr tts);

        [DllImport("TTSDLL", EntryPoint = "GetNowVoice", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr GetNowVoice(IntPtr tts);

        [DllImport("TTSDLL", EntryPoint = "GetVoiceByNum", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr GetVoiceByNum(IntPtr tts, int num);

        [DllImport("TTSDLL", EntryPoint = "GetErrorMessage", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr GetErrorMessage(IntPtr tts);

        [DllImport("TTSDLL", EntryPoint = "SetVoiceByName", CallingConvention = CallingConvention.Cdecl)]
        static extern int SetVoiceByName(IntPtr tts, [MarshalAs(UnmanagedType.LPStr)] string name);

        [DllImport("TTSDLL", EntryPoint = "SetVoiceByNum", CallingConvention = CallingConvention.Cdecl)]
        static extern int SetVoiceByNum(IntPtr tts, int num);

        [DllImport("TTSDLL", EntryPoint = "SetOutputPath", CallingConvention = CallingConvention.Cdecl)]
        static extern int SetOutputPath(IntPtr tts, [MarshalAs(UnmanagedType.LPStr)] string filePath);


        [DllImport("TTSDLL", EntryPoint = "TextToSpeech", CallingConvention = CallingConvention.Cdecl)]
        static extern int TextToSpeech(IntPtr tts, [MarshalAs(UnmanagedType.LPStr)] string text, bool output = false);

        private IntPtr _speechHandle;
        private AudioSource _audioSource;
        private string _ttsFilePath = Application.streamingAssetsPath + "/";
        private string _ttsFileName = "tts_cache.wav";

        private void Awake()
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
            _speechHandle = CreateTTS();
            SetPath(_ttsFilePath, _ttsFileName);
        }

        public string GetVoice(int num = -1)
        {
            IntPtr ptr;
            string str;
            if (num < 0)
                ptr = GetNowVoice(_speechHandle);
            else
                ptr = GetVoiceByNum(_speechHandle, num);
            str = Marshal.PtrToStringUni(ptr);
            Marshal.FreeHGlobal(ptr);
            return str;
        }

        public string[] GetVoiceNames()
        {
            var count = GetVoiceCount(_speechHandle);
            var names = new List<string>();
            IntPtr ptr;

            for (var i = 0; i < count; i++)
            {
                ptr = GetVoiceByNum(_speechHandle, i);
                names.Add(Marshal.PtrToStringUni(ptr));
                Marshal.FreeHGlobal(ptr);
            }

            return names.ToArray();
        }

        public void Speech(string text)
        {
            if (TextToSpeech(_speechHandle, text, true) == 0)
            {
                Debug.Log(GetMessage());
            }

            PlayTTS();
        }

        private void SetPath(string path, string file)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if (SetOutputPath(_speechHandle, path + file) == FALSE)
            {
                Debug.Log("fail to set output:" + GetMessage());
            }
        }

        public void SetVoice(int num)
        {
            if (num < 0) return;

            if (SetVoiceByNum(_speechHandle, num) == FALSE)
            {
                Debug.Log(GetMessage());
            }
        }

        public void SetVoice(string voiceName)
        {
            if (voiceName.Length <= 0) return;

            if (SetVoiceByName(_speechHandle, voiceName) == FALSE)
            {
                Debug.Log(GetMessage());
            }
        }

        private string GetMessage()
        {
            var handle = GetErrorMessage(_speechHandle);
            var error = Marshal.PtrToStringUni(handle);
            Marshal.FreeHGlobal(handle);
            return error;
        }

        public void PlayTTS()
        {
            EnsureClipDisposed();

            // Load Clip then assign to audio source and play
            StartCoroutine(LoadClipCoroutine("file:///" + _ttsFilePath + _ttsFileName));
        }
        
        IEnumerator LoadClipCoroutine(string file)
        {
            var request = new WWW(file);
            yield return request;

            var clip = request.GetAudioClip(true, true);
            _audioSource.clip = clip;
            _audioSource.Play();

            request.Dispose();
        }

        private void EnsureClipDisposed()
        {
            if (_audioSource.clip == null) return;

            if (_audioSource.isPlaying)
                _audioSource.Stop();
            
            var clip = _audioSource.clip;
            _audioSource.clip = null;
            
            clip.UnloadAudioData();
            Destroy(clip); // This is important to avoid memory leak
        }
    }
}