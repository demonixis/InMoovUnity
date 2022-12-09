using Demonixis.InMoov.Chatbots;
using Demonixis.InMoov.Services.Speech;
using Demonixis.InMoov.Servos;
using Demonixis.InMoov.Settings;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Demonixis.InMoov
{
    public class Robot : MonoBehaviour
    {
        private static Robot _instance;
        private const string ServiceListFilename = "services.json";
        private const string SystemListFilename = "systems.json";

        private List<RobotService> _currentServices;
        private ServiceList _serviceList;
        private List<Action> _waitingStartCallbacks;

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
        public ServoMixerService ServoMixer => _servoMixerService;

        /// <summary>
        /// Retrieve an array of active services.
        /// </summary>
        public RobotService[] Services => _currentServices?.ToArray() ?? Array.Empty<RobotService>();

        public bool Running { get; private set; }

        public event Action<Robot> Initialized;
        public event Action<RobotService, RobotService> ServiceChanged;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                enabled = false;
                Debug.LogWarning($"Double instance detected {name}");
                return;
            }

            _currentServices = new List<RobotService>();
            _waitingStartCallbacks = new List<Action>();
        }

        private void Start()
        {
            // TODO in prevision of deferred load.
            InitializeServices();

            Running = true;
            Initialized?.Invoke(this);
            
            InitializeSystems();
            
            if (_waitingStartCallbacks.Count <= 0) return;
            foreach (var callback in _waitingStartCallbacks)
                callback?.Invoke();
        }

        public void OnStarted(Action callback)
        {
            if (Running)
                callback?.Invoke();
            else
                _waitingStartCallbacks.Add(callback);
        }

        public T GetServiceOfType<T>() where T : RobotService
        {
            var type = typeof(T);
            foreach (var current in Services)
            {
                if (current is T service)
                    return service;
            }

            Debug.Log($"Service {type} was not found. It's not really possible...");
            return null;
        }

        public void ChangeService<T>(T service) where T : RobotService
        {
            // FIXME
            T old = default(T);
            T newService = default(T);
            ServiceChanged?.Invoke(old, newService);
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

            _voiceRecognition.PhraseDetected += s => { _chatbotService.SubmitResponse(s); };

            _servoMixerService.Initialize();
        }

        private void InitializeSystems()
        {
            var systemsList =
                SaveGame.LoadRawData<string[]>(SaveGame.GetPreferredStorageMode(), SystemListFilename, "Config");

            if (systemsList == null || systemsList.Length == 0) return;

            var systems = GetComponentsInChildren<RobotSystem>(true);

            foreach (var system in systems)
            {
                if (Array.IndexOf(systemsList, system.GetType().Name) == -1) continue;
                system.SetActive(true);
            }
        }

        private void OnDestroy()
        {
            // Save the list of used services
            _serviceList.Chatbot = _chatbotService.GetType().Name;
            _serviceList.VoiceRecognition = _voiceRecognition.GetType().Name;
            _serviceList.SpeechSynthesis = _speechSynthesis.GetType().Name;
            _serviceList.ServoMixer = _servoMixerService.GetType().Name;

            SaveGame.SaveRawData(SaveGame.GetPreferredStorageMode(), _serviceList, ServiceListFilename, "Config");

            ClearCurrentServices();

            // Save the list of used systems
            var activeSystems = new List<string>();
            var systems = GetComponentsInChildren<RobotSystem>();
            foreach (var system in systems)
            {
                if (!system.Started) continue;
                activeSystems.Add(system.GetType().Name);
                system.SetActive(false);
            }

            SaveGame.SaveRawData(SaveGame.GetPreferredStorageMode(), activeSystems.ToArray(), SystemListFilename,
                "Config");
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