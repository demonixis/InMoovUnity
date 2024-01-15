using Demonixis.InMoovSharp;
using Demonixis.InMoovSharp.Services;
using Demonixis.InMoovSharp.Utils;
using Demonixis.InMoovUnity.Services;
using System;
using System.Collections;
using UnityEngine;

namespace Demonixis.InMoovUnity
{
    [RequireComponent(typeof(AudioSource))]
    public class UnityRobot : MonoBehaviour
    {
        private static Robot _nativeRobot;
        private static UnityRobot _unityRobot;
        private static bool Running;

        public static UnityRobot Instance
        {
            get
            {
                if (_unityRobot == null)
                {
                    _unityRobot = FindFirstObjectByType<UnityRobot>();

                    if (_unityRobot == null)
                        throw new UnityException("The Robot Instance doesn't exists!");
                }

                return _unityRobot;
            }
        }

        public Robot NativeRobot => _nativeRobot;

        [SerializeField] private bool _autoStart = true;
        [SerializeField] private int _frameRate = 30;
        [SerializeField] private float _refreshRate = 30.0f;

        private event Action<UnityRobot> UnityRobotReady = null;

        private void Awake()
        {
            _nativeRobot = new Robot();
            _nativeRobot.LogEnabled = false;
        }

        public void OnRobotReady(Action<UnityRobot> robot)
        {
            if (Running)
                robot?.Invoke(this);
            else
                UnityRobotReady += robot;
        }

        private void Start()
        {
            Application.targetFrameRate = _frameRate;

            if (_autoStart)
                InitializeRobot();
        }

        private void InitializeRobot()
        {
            _nativeRobot.InitializeRobot();
            _nativeRobot.AddService(new UnityComputerVision());

            var voiceRSS = new VoiceRssTTS();
            _nativeRobot.AddService(voiceRSS);

            var vosk = new VoskSpeechRecognition();
            _nativeRobot.AddService(vosk);
            _nativeRobot.SwapServices<VoiceRecognitionService>(vosk);

#if UNITY_STANDALONE_WIN
            var msTTS = new MicrosoftTTS();
            _nativeRobot.SwapServices<SpeechSynthesisService>(msTTS);
#elif UNITY_STANDALONE_MAC
            var macTTS = new MacosTTS();
            _nativeRobot.AddService(macTTS);
            _nativeRobot.SwapServices<SpeechSynthesisService>(macTTS);
#else
            _nativeRobot.SwapServices<SpeechSynthesisService>(voiceRSS);
#endif

            _nativeRobot.LogEnabled = true;

            StartCoroutine(RobotLoop());
        }

        private IEnumerator RobotLoop()
        {
            var refreshRate = 1.0f / _refreshRate;

            Running  = true;

            UnityRobotReady?.Invoke(this);

            while (Running)
            {
                _nativeRobot.UpdateRobot();
                yield return CoroutineFactory.WaitForSeconds(refreshRate);
            }
        }
    }
}
