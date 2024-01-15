#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
#define MS_SPEECH_SYNTHESIS
#endif
using Demonixis.InMoovSharp.Utils;
using MSSpeechLink;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Demonixis.InMoovUnity
{
    public class SpeechLink : MonoBehaviour
    {
        private static SpeechLink _instance;
#if MS_SPEECH_SYNTHESIS
        private static object _locker = new();
        private const string ProcessName = "MSSpeechLink";
        private WebSocketSharp.WebSocket _websocket;
        private Coroutine _coroutine;
        private Process _speechLinkProcess;
        private bool _needReconnection;
        private List<MessageData> _messageQueue;
        private bool _checkMessages;
        private bool _enableVoiceRecongition;
#endif
        [SerializeField] private bool _bypassProcessStart = false;

        public static SpeechLink Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<SpeechLink>();
                    if (_instance == null)
                    {
                        var go = new GameObject("WindowsSpeechWebSocket");
                        _instance = go.AddComponent<SpeechLink>();
                    }
                }

                return _instance;
            }
        }

        public string[] Voices { get; private set; }
        public bool IsSpeaking { get; private set; }
        public int VoiceIndex { get; private set; }

        public bool EnableVoiceRecognition
        {
#if MS_SPEECH_SYNTHESIS
            get => _enableVoiceRecongition;
            set
            {
                _enableVoiceRecongition = value;
                SendMessage(MessageType.EnableVoiceRecognition, (value ? 1 : 0).ToString());
            }
#else
            get; set;
#endif
        }

        public event Action<string> VoiceRecognized;
        public event Action<string[]> VoicesReceived;

#if MS_SPEECH_SYNTHESIS

        #region Unity Pattern

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Debug.LogError($"Only one instance of WindowsSpeechWebSocket is allowed. Destroying {name}");
                Destroy(this);
            }
        }

        public void Initialize()
        {
            if (_messageQueue != null) return;

            _messageQueue = new List<MessageData>();
            StartCoroutine(TryJoinWebSocketServerCoroutine());
            StartCoroutine(CheckMessageQueue());
        }

        private void OnDestroy()
        {
            if (_speechLinkProcess != null)
                _speechLinkProcess.Kill();

            StopAllCoroutines();
            StopWebSocketConnection();
        }

        #endregion

        #region Public API

        public void SetVoice(string voice)
        {
            // VoiceName#Culture
            if (voice.Contains("#"))
                voice = voice.Split('#')[0];

            SendMessage(MessageType.SetVoice, voice);
        }

        public void SetVoice(int voiceId)
        {
            SendMessage(MessageType.SetVoiceByIndex, voiceId.ToString());
        }

        public void SetLanguage(string lang)
        {
            SendMessage(MessageType.SetLanguage, lang);
        }

        public void Speak(string words)
        {
            IsSpeaking = true;
            SendMessage(MessageType.Speak, words);
        }

        #endregion

        #region Messages Management

        private IEnumerator CheckMessageQueue()
        {
            while (true)
            {
                if (_needReconnection)
                {
                    TryJoinWebSocketServer();
                    _needReconnection = false;
                }

                if (_checkMessages)
                {
                    lock (_locker)
                    {
                        if (_messageQueue.Count > 0)
                        {
                            foreach (var data in _messageQueue)
                            {
                                switch (data.MessageType)
                                {
                                    case MessageType.VoiceRecognitionResult:
                                        VoiceRecognized?.Invoke(data.Message);
                                        break;

                                    case MessageType.GetVoices:
                                        // VoiceIndex_VoiceName#Culture|VoiceName1#Culture
                                        var tmp = data.Message.Split('_');
                                        if (tmp.Length != 2)
                                        {
                                            Debug.LogError($"[SpeechLink] Voice data has not the correct size.");
                                            break;
                                        }

                                        var voiceIndex = tmp[0];
                                        var voices = tmp[1];

                                        VoiceIndex = int.Parse(voiceIndex);
                                        Voices = voices.Split('|');
                                        VoicesReceived?.Invoke(Voices);
                                        break;
                                    case MessageType.SpeakStart:
                                        IsSpeaking = true;
                                        break;
                                    case MessageType.SpeakEnd:
                                        IsSpeaking = false;
                                        break;
                                }
                            }

                            _messageQueue.Clear();
                        }
                    }

                    _checkMessages = false;
                }

                yield return CoroutineFactory.WaitForSeconds(1.0f);
            }
        }

        private void SendMessage(MessageType type, string data)
        {
            var json = JsonConvert.SerializeObject(new MessageData
            {
                MessageType = type,
                Message = data
            });

            if (_websocket != null && _websocket.IsAlive)
                _websocket.Send(json);
        }

        #endregion

        #region WebSocket Management

        private void StartWebSocketConnection()
        {
            _websocket = new WebSocketSharp.WebSocket("ws://127.0.0.1:8831");
            _websocket.ConnectAsync();
            _websocket.OnClose += _websocket_OnClose;
            _websocket.OnError += _websocket_OnError;
            _websocket.OnOpen += _websocket_OnOpen;
            _websocket.OnMessage += _websocket_OnMessage;
        }

        private void StopWebSocketConnection()
        {
            if (_websocket is { IsAlive: true })
                _websocket.Close();
        }

        private void TryJoinWebSocketServer(bool force = false)
        {
            if (_coroutine == null || force)
                _coroutine = StartCoroutine(TryJoinWebSocketServerCoroutine());
            else
                Debug.Log("Still trying to join the Server");
        }

        private IEnumerator TryJoinWebSocketServerCoroutine()
        {
#if !UNITY_EDITOR
        _bypassProcessStart = false;
#endif
            if (!IsSpeechLinkStarted() && !_bypassProcessStart)
            {
                var exec = Path.Combine(Application.streamingAssetsPath, "ThirdParty",
                    "SpeechLink", ProcessName);
                _speechLinkProcess = new Process();
                _speechLinkProcess.StartInfo.FileName = exec;
                _speechLinkProcess.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                _speechLinkProcess.Start();

                yield return new WaitForSeconds(0.5f);
            }

            StopWebSocketConnection();
            StartWebSocketConnection();

            yield return new WaitForSeconds(2.5f);

            _coroutine = null;

            if (_websocket.IsAlive) yield break;

            TryJoinWebSocketServer();
        }

        #endregion

        #region Process Management

        private bool IsSpeechLinkStarted()
        {
            var process = Process.GetProcesses();
            foreach (var p in process)
            {
                if (p.ProcessName == ProcessName)
                    return true;
            }

            return false;
        }

        #endregion

        #region Event Handlers

        private void _websocket_OnError(object sender, WebSocketSharp.ErrorEventArgs e)
        {
            Debug.Log($"WebSocket Error: {e.Message}");
            _needReconnection = true;
        }

        private void _websocket_OnClose(object sender, WebSocketSharp.CloseEventArgs e)
        {
            Debug.Log($"WebSocket Closed: {e.Reason}");
            _needReconnection = true;
        }

        private void _websocket_OnOpen(object sender, System.EventArgs e)
        {
            Debug.Log("Connected to the websocket Speech server.");
        }

        private void _websocket_OnMessage(object sender, WebSocketSharp.MessageEventArgs e)
        {
            if (!e.IsText) return;

            var result = Encoding.UTF8.GetString(e.RawData);

            if (string.IsNullOrEmpty(result)) return;

            var data = JsonConvert.DeserializeObject<MessageData>(result);

            lock (_locker)
                _messageQueue.Add(data);

            _checkMessages = true;
        }

        #endregion

#endif
    }
}