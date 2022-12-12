using Demonixis.InMoov.Chatbots;
using Demonixis.InMoov.Services.Speech;
using Demonixis.InMoov.Servos;
using Demonixis.InMoov.Settings;
using System;
using System.Collections.Generic;
using Demonixis.InMoov.ComputerVision;
using Demonixis.InMoov.Navigation;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Demonixis.InMoov
{
    public sealed class Robot : MonoBehaviour
    {
        private static Robot _instance;
        private const string ServiceListFilename = "services.json";
        private const string SystemListFilename = "systems.json";

        private SpeechBrainProxy _speechBrainProxy;
        private List<RobotService> _currentServices;
        private List<Action> _waitingStartCallbacks;

        [SerializeField] private bool _autoStartRobot = true;

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

        /// <summary>
        /// Retrieve an array of active services.
        /// </summary>
        public RobotService[] Services => _currentServices?.ToArray() ?? Array.Empty<RobotService>();

        /// <summary>
        /// Retrieve an array of all services, active or inactive, available.
        /// </summary>
        public RobotService[] AllServices => FindObjectsOfType<RobotService>(true);

        public bool Started { get; private set; }

        public event Action<Robot> RobotInitialized;
        public event Action<RobotService, RobotService> ServiceChanged;
        public event Action<RobotService, bool> ServicePaused;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                enabled = false;
                Debug.LogWarning($"Double instance detected {name}");
                return;
            }

            _speechBrainProxy = new SpeechBrainProxy();
            _currentServices = new List<RobotService>();
            _waitingStartCallbacks = new List<Action>();
        }

        private void Start()
        {
            if (_autoStartRobot)
                InitializeRobot();
        }

        public void InitializeRobot()
        {
            // TODO in prevision of deferred load.
            InitializeServices();

            Started = true;
            RobotInitialized?.Invoke(this);

            InitializeSystems();

            if (_waitingStartCallbacks.Count <= 0) return;
            foreach (var callback in _waitingStartCallbacks)
                callback?.Invoke();

            _waitingStartCallbacks.Clear();
        }

        public void WhenStarted(Action callback)
        {
            if (Started)
                callback?.Invoke();
            else
                _waitingStartCallbacks.Add(callback);
        }

        #region Service Management

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
            var chatbotService = SelectService<ChatbotService>(serviceList.Chatbot);
            var voiceRecognition = SelectService<VoiceRecognitionService>(serviceList.VoiceRecognition);
            var speechSynthesis = SelectService<SpeechSynthesisService>(serviceList.SpeechSynthesis);
            SelectService<ServoMixerService>(serviceList.ServoMixer);
            SelectService<ServoMixerService>(serviceList.XR);
            SelectService<ServoMixerService>(serviceList.Navigation);
            SelectService<ServoMixerService>(serviceList.ComputerVision);

            _speechBrainProxy.Setup(chatbotService, voiceRecognition, speechSynthesis);
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
                throw new UnityException(
                    $"Service {targetService} is not available and there is no suitable service found.");
            }

            _currentServices.Add(selectedService);

            selectedService.Initialize();

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

        /// <summary>
        /// Gets the active service of type T.
        /// </summary>
        /// <typeparam name="T">The type of serice.</typeparam>
        /// <returns>It returns a service.</returns>
        public T GetService<T>() where T : RobotService
        {
            var type = typeof(T);
            foreach (var current in _currentServices)
            {
                if (current is T service)
                    return service;
            }

            Debug.Log($"Service {type} was not found. It's not really possible...");
            return null;
        }

        public bool TryGetService<T>(out T outService) where T : RobotService
        {
            var type = typeof(T);
            foreach (var current in _currentServices)
            {
                if (current is not T service) continue;
                outService = service;
                return true;
            }

            Debug.Log($"Service {type} was not found. It's not really possible...");
            outService = null;
            return false;
        }

        /// <summary>
        /// Change a service by another one using a name.
        /// </summary>
        /// <typeparam name="T">The type of service</typeparam>
        /// <param name="serviceName">The new service name</param>
        public void ReplaceService<T>(string serviceName) where T : RobotService
        {
            if (TryFindServiceByName(serviceName, out T newService))
            {
                var oldService = GetService<T>();
                oldService.Shutdown();
                _currentServices.Remove(oldService);

                newService.Initialize();
                _currentServices.Add(newService);

                ServiceChanged?.Invoke(oldService, newService);

                // FIXME For now we'll reload the scene because we need
                // to implement this change everywhere. It'll be made soon
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            else
                Debug.LogError($"Robot::ReplaceService - I wasn't able to find the service {serviceName}.");
        }

        /// <summary>
        /// Try to find a service using its name.
        /// </summary>
        /// <param name="serviceName">The sercice name</param>
        /// <param name="outService">The service instance</param>
        /// <typeparam name="T">It returns true if the service was found, otherwise it returns false.</typeparam>
        /// <returns></returns>
        private bool TryFindServiceByName<T>(string serviceName, out T outService) where T : RobotService
        {
            var services = GetComponentsInChildren<T>();

            foreach (var newService in services)
            {
                if (newService.ServiceName != serviceName) continue;
                outService = newService;
                return true;
            }

            outService = null;
            return false;
        }

        /// <summary>
        /// Pause a service by its type.
        /// </summary>
        /// <typeparam name="T">The type of service.</typeparam>
        /// <param name="paused">Set to true to pause the service and false to unpause it.</param>
        public void SetServicePaused<T>(bool paused) where T : RobotService
        {
            if (!TryGetService(out T service)) return;
            service.SetPaused(paused);
            ServicePaused?.Invoke(service, paused);
        }

        #endregion

        #region Systems Management

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

        #endregion

        private void OnDestroy()
        {
            var serviceList = new ServiceList();

            foreach (var service in _currentServices)
            {
                if (service is ChatbotService)
                    serviceList.Chatbot = service.ServiceName;
                else if (service is VoiceRecognitionService)
                    serviceList.VoiceRecognition = service.ServiceName;
                else if (service is SpeechSynthesisService)
                    serviceList.SpeechSynthesis = service.ServiceName;
                else if (service is ServoMixerService)
                    serviceList.ServoMixer = service.ServiceName;
                else if (service is NavigationService)
                    serviceList.Navigation = service.ServiceName;
                else if (service is ComputerVisionService)
                    serviceList.ComputerVision = service.ServiceName;
            }

            // Save the list of used services
            SaveGame.SaveRawData(SaveGame.GetPreferredStorageMode(), serviceList, ServiceListFilename, "Config");

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

            GlobalSettings.Save();
        }
    }
}