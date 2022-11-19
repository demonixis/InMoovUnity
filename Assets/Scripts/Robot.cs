using System;
using System.Collections.Generic;
using Demonixis.InMoov.Chatbots;
using Demonixis.InMoov.Servos;
using Demonixis.InMoov.Settings;
using Demonixis.InMoov.Speech;
using Demonixis.ToolboxV2;
using UnityEngine;

namespace Demonixis.InMoov
{
    public class Robot : MonoBehaviour
    {
        private static Robot _instance;
        private const string ServiceListFilename = "services.json";

        private List<RobotService> _currentServices;
        private ServiceList _serviceList;

        // Human Understanding
        private ChatbotService _chatbotService;
        private SpeechSynthesisService _speechSynthesis;
        private VoiceRecognitionService _voiceRecognition;

        // Animation
        private ServoMixerService _servoMixerService;

        /// <summary>
        /// Gets a static instance of the robot.
        /// </summary>
        public static Robot Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<Robot>();

                    if (_instance == null)
                        Debug.LogException(new UnityException("Robot is missing."));
                }

                return _instance;
            }
        }

        public ChatbotService Chatbot => _chatbotService;
        public VoiceRecognitionService VoiceRecognition => _voiceRecognition;
        public SpeechSynthesisService SpeechSynthesis => _speechSynthesis;

        /// <summary>
        /// Retrieve an array of active services.
        /// </summary>
        public RobotService[] Services => _currentServices?.ToArray() ?? Array.Empty<RobotService>();

        private void Awake()
        {
            _currentServices = new List<RobotService>();
        }

        private void Start()
        {
            InitializeServices();
        }

        /// <summary>
        /// Reboot services by clearing then reinitializing them
        /// </summary>
        public void RebootRebot()
        {
            ClearCurrentServices();
            InitializeServices();
        }

        /// <summary>
        /// Initialize services to make the robot alive
        /// </summary>
        private void InitializeServices()
        {
            var serviceList =
                SaveGame.LoadRawData<ServiceList>(SaveGame.GetPreferredStorageMode(), ServiceListFilename, "Config");

            if (!serviceList.IsValid())
                serviceList = ServiceList.New();

            // Select service selected by the user
            _chatbotService = SelectService<ChatbotService>(serviceList.Chatbot);
            _voiceRecognition = SelectService<VoiceRecognitionService>(serviceList.VoiceRecognition);
            _speechSynthesis = SelectService<SpeechSynthesisService>(serviceList.SpeechSynthesis);
            _servoMixerService = SelectService<ServoMixerService>(serviceList.ServoMixer);

            // Initialize them
            _voiceRecognition.Initialize();
            _speechSynthesis.Initialize();
            _chatbotService.Initialize();
            _chatbotService.ResponseReady += response =>
            {
                _speechSynthesis.Speak(string.IsNullOrEmpty(response) ? "I don't understand" : response);
            };

            _voiceRecognition.PhraseDetected += s =>
            {
                _chatbotService.SubmitResponse(s);
            };

            _servoMixerService.Initialize();
        }

        private void OnDestroy()
        {
            _serviceList.Chatbot = _chatbotService.GetType().Name;
            _serviceList.VoiceRecognition = _voiceRecognition.GetType().Name;
            _serviceList.SpeechSynthesis = _speechSynthesis.GetType().Name;
            _serviceList.ServoMixer = _servoMixerService.GetType().Name;
            SaveGame.SaveRawData(SaveGame.GetPreferredStorageMode(), _serviceList, ServiceListFilename, "Config");

            ClearCurrentServices();
        }

        /// <summary>
        /// Select the service T by its name and use a fallback if not available
        /// </summary>
        /// <param name="targetService">The service name</param>
        /// <typeparam name="T">The service type</typeparam>
        /// <returns>It returns a RobotService</returns>
        private T SelectService<T>(string targetService) where T : RobotService
        {
            var services = GetComponentsInChildren<T>();
            T selectedService = null;

            foreach (var service in services)
            {
                if (service.ServiceName == targetService && service.IsSupported())
                    selectedService = service;

                if (selectedService == null && service.IsSupported())
                    selectedService = service;
            }

            if (selectedService == null)
            {
                Debug.LogException(
                    new UnityException(
                        $"Service {targetService} is not available and there is no suitable service found."));
            }

            _currentServices.Add(selectedService);

            Debug.Log($"[{typeof(T)} service: {selectedService}");

            return selectedService;
        }

        /// <summary>
        /// Shutdown loaded services and free the resources.
        /// </summary>
        private void ClearCurrentServices()
        {
            if (_currentServices.Count <= 0) return;

            foreach (var service in _currentServices)
                service.Shutdown();

            _currentServices.Clear();
        }
    }
}