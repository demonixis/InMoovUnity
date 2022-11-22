using MSSpeechLink;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;

namespace Demonixis.InMoov.Services.Speech
{
    public class SpeechLink : MonoBehaviour
    {
        private static SpeechLink _instance;
        private const string ProcessName = "MSSpeechLink.exe";
        private WebSocketSharp.WebSocket _websocket;
        private Coroutine _coroutine;
        private bool _processStarted;

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

        public event Action<string> VoiceRecognized;
        public event Action<string[]> VoicesReceived;

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
            StartCoroutine(TryJoinWebSocketServerCoroutine());
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
            StopWebSocketConnection();
        }

        public void SelectVoice(string voice)
        {
            // VoiceName#Culture
            if (voice.Contains("#"))
                voice = voice.Split('#')[0];

            SendMessage(MessageType.SelectVoice, voice);
        }

        public void SelectLang(string lang)
        {
            SendMessage(MessageType.SelectLang, lang);
        }

        public void Speak(string words)
        {
            SendMessage(MessageType.TextToSpeech, words);
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

        private void _websocket_OnError(object sender, WebSocketSharp.ErrorEventArgs e)
        {
            Debug.Log("Error on the websocket Speech server.");
            if (enabled)
                TryJoinWebSocketServer();
        }

        private void _websocket_OnClose(object sender, WebSocketSharp.CloseEventArgs e)
        {
            Debug.Log("Websocket Speech server closed.");
            if (enabled)
                TryJoinWebSocketServer();
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

            switch (data.MessageType)
            {
                case MessageType.VoiceRecognized:
                    VoiceRecognized?.Invoke(data.Message);
                    break;

                case MessageType.ListVoices:
                {
                    var voices = data.Message.Split('|');
                    VoicesReceived?.Invoke(voices);
                }
                    break;
            }
        }

        private void TryJoinWebSocketServer(bool force = false)
        {
            if (_coroutine == null || force)
                _coroutine = StartCoroutine(TryJoinWebSocketServerCoroutine());
        }

        private IEnumerator TryJoinWebSocketServerCoroutine()
        {
            StopWebSocketConnection();
            StartWebSocketConnection();

            yield return new WaitForSeconds(2.5f);

            _coroutine = null;

            if (!_websocket.IsAlive)
            {
                if (!_processStarted)
                {
                    System.Diagnostics.Process.Start(Path.Combine(Application.streamingAssetsPath, "ThirdParty",
                        "SpeechLink", ProcessName));
                    _processStarted = true;
                    yield return new WaitForSeconds(0.5f);
                }

                TryJoinWebSocketServer();
            }
        }
    }
}