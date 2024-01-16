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
    public class UnityRobotProxy : MonoBehaviour
    {
        private static Robot _robot;
        private static UnityRobotProxy _robotProxy;
        private static bool Running;

        public static UnityRobotProxy Instance
        {
            get
            {
                if (_robotProxy == null)
                {
                    _robotProxy = FindFirstObjectByType<UnityRobotProxy>();

                    if (_robotProxy == null)
                        throw new UnityException("The Robot Instance doesn't exists!");
                }

                return _robotProxy;
            }
        }

        public Robot Robot => _robot;

        [SerializeField] private bool _autoStart = true;
        [SerializeField] private int _frameRate = 30;
        [SerializeField] private float _refreshRate = 30.0f;

        private event Action<UnityRobotProxy> UnityRobotReady = null;

        private void Awake()
        {
            _robot = new Robot();
            _robot.LogEnabled = false;
        }

        private void Start()
        {
            Application.targetFrameRate = _frameRate;

            if (_autoStart)
                InitializeRobot();
        }

        private void OnApplicationQuit()
        {
            _robot?.Dispose();
        }

        private void InitializeRobot()
        {
            _robot.ServicePreInit += RegisterServices;
            _robot.SystemPreInit += RegisterSystems;
            _robot.InitializeRobot();
            _robot.LogEnabled = true;

            StartCoroutine(RobotLoop());
        }

        private void RegisterServices(Robot robot)
        {
            var cv = new UnityComputerVision();
            _robot.AddService(cv);
            _robot.SwapServices<ComputerVisionService>(cv);

            var samTts = new SamTTS();
            _robot.AddService(samTts);
            _robot.SwapServices<SpeechSynthesisService>(samTts);

            var voiceRss = new VoiceRssTTS();
            _robot.AddService(voiceRss);

#if UNITY_STANDALONE || UNITY_ANDROID
            var vosk = new VoskSpeechRecognition();
            _robot.AddService(vosk);
            _robot.SwapServices<VoiceRecognitionService>(vosk);
#endif
#if UNITY_STANDALONE_WIN
            var msTts = new MicrosoftTTS();
            _robot.AddService(msTts);
            _robot.SwapServices<SpeechSynthesisService>(msTts);
#elif UNITY_STANDALONE_MAC
            var macTTS = new MacosTTS();
            _nativeRobot.AddService(macTTS);
            _nativeRobot.SwapServices<SpeechSynthesisService>(macTTS);
#endif
        }

        private void RegisterSystems(Robot robot)
        {
            // Nothing for now
        }

        private IEnumerator RobotLoop()
        {
            var refreshRate = 1.0f / _refreshRate;

            Running  = true;

            UnityRobotReady?.Invoke(this);
            UnityRobotReady = null;

            while (Running)
            {
                _robot.UpdateRobot();
                yield return CoroutineFactory.WaitForSeconds(refreshRate);
            }
        }

        public void OnRobotReady(Action<UnityRobotProxy> robot)
        {
            if (Running)
                robot?.Invoke(this);
            else
                UnityRobotReady += robot;
        }
    }
}
