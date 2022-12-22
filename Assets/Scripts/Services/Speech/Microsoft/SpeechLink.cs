using MSSpeechLink;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Demonixis.InMoov.Utils;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Demonixis.InMoov.Services.Speech
{
    public class SpeechLink : MonoBehaviour
    {
        private static SpeechLink _instance;
        private static object _locker = new ();
        private const string ProcessName = "MSSpeechLink";
        private WebSocketSharp.WebSocket _websocket;
        private Coroutine _coroutine;
        private Process _speechLinkProcess;
        private bool _needReconnection;
        private List<MessageData> _messageQueue;

        public static SpeechLink Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<SpeechLink>();
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

        public event Action<string> VoiceRecognized;
        public event Action<string[]> VoicesReceived;

        #region Unity Pattern
        
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Debug.LogError($"Only one instance of WindowsSpeechWebSocket is allowed. Destroying {name}");
                Destroy(this);
                return;
            }

            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
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

        private IEnumerator CheckMessageQueue()
        {
            while (true)
            {
                if (_needReconnection)
                {
                    TryJoinWebSocketServer();
                    _needReconnection = false;
                }
                
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
                                    Voices = data.Message.Split('|');
                                    VoicesReceived?.Invoke(Voices);
                                    break;
                            }
                        }

                        _messageQueue.Clear();
                    }
                }

                yield return CoroutineFactory.WaitForSeconds(1.0f);
            }
        }
        
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
            SendMessage(MessageType.Speak, words);
        }
        
        #endregion

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
            if (_websocket is {IsAlive: true})
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
            if (!IsSpeechLinkStarted())
            {
                var exec = Path.Combine(Application.streamingAssetsPath, "ThirdParty",
                    "SpeechLink", ProcessName);
                _speechLinkProcess = new Process();
                _speechLinkProcess.StartInfo.FileName = exec;
                _speechLinkProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
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
        }
        
        #endregion
    }
}