using Demonixis.InMoovSharp.Services;
using Demonixis.InMoovUnity;
using Demonixis.InMoovUnity.Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class ServicePanel : MonoBehaviour
{
    [Header("Brain")] [SerializeField] private TMP_Dropdown _botServiceList;
    [SerializeField] private Toggle _botServiceStatus;
    [SerializeField] private TMP_Dropdown _speechRecognitionServiceList;
    [SerializeField] private Toggle _speechRecognitionServiceStatus;
    [SerializeField] private TMP_Dropdown _speechSynthesisServiceList;
    [SerializeField] private TMP_Dropdown _speechSynthesisVoiceList;
    [SerializeField] private Toggle _speechSynthesisServiceStatus;

    [Header("Body")] [SerializeField] private TMP_Dropdown _servoMixerServiceList;
    [SerializeField] private Toggle _servoMixerServiceStatus;
    [SerializeField] private TMP_Dropdown _navigationServiceList;
    [SerializeField] private Toggle _navigationServiceStatus;

    [Header("Perception")] [SerializeField]
    private TMP_Dropdown _computerVisionServiceList;

    [SerializeField] private Toggle _computerVisionServiceStatus;

    private void Start()
    {
        UnityRobotProxy.Instance.OnRobotReady(InternalInitialize);
    }

    private void InternalInitialize(UnityRobotProxy unityRobot)
    {
        SetupService<ChatbotService>(_botServiceList, _botServiceStatus);
        SetupService<SpeechSynthesisService>(_speechSynthesisServiceList, _speechSynthesisServiceStatus);
        SetupService<VoiceRecognitionService>(_speechRecognitionServiceList, _speechRecognitionServiceStatus);
        SetupService<NavigationService>(_navigationServiceList, _navigationServiceStatus);
        SetupService<ComputerVisionService>(_computerVisionServiceList, _computerVisionServiceStatus);
        SetupService<ServoMixerService>(_servoMixerServiceList, _servoMixerServiceStatus);

        SetupSpeechVoices();
    }

    private void SetupSpeechVoices()
    {
        var robot = UnityRobotProxy.Instance.Robot;
        var speechSynthesis = robot.GetService<SpeechSynthesisService>();
        var voices = speechSynthesis.GetVoices();

        _speechSynthesisVoiceList.options.Clear();

        if (voices == null || voices.Length == 0)
        {
            if (speechSynthesis is MicrosoftTTS)
            {
                SpeechLink.Instance.VoicesReceived += _ =>
                {
                    SetupSpeechVoices();
                };
            }
            return;
        }

        foreach (var voice in voices)
            _speechSynthesisVoiceList.options.Add(new TMP_Dropdown.OptionData(voice));

        var selectedIndex = speechSynthesis.GetVoiceIndex();
        if (selectedIndex > -1)
        {
            _speechSynthesisVoiceList.SetValueWithoutNotify(selectedIndex);
            _speechSynthesisVoiceList.RefreshShownValue();
        }

        _speechSynthesisVoiceList.onValueChanged.AddListener(i =>
        {
            var service = robot.GetService<SpeechSynthesisService>();
            service.SetVoice(i);
        });
    }

    private void SetupService<T>(TMP_Dropdown dropdown, Toggle toggle) where T : RobotService
    {
        var robot = UnityRobotProxy.Instance.Robot;
        var services = robot.GetServicesOfType<T>();
        var activatedIndex = 0;

        dropdown.options.Clear();

        for (var i = 0; i < services.Length; i++)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData(services[i].ServiceName));

            if (services[i].Started)
                activatedIndex = i;
        }

        dropdown.SetValueWithoutNotify(activatedIndex);
        dropdown.RefreshShownValue();
        dropdown.onValueChanged.AddListener(i =>
        {
            var serviceName = dropdown.options[dropdown.value].text;
            robot.ReplaceService<T>(serviceName);
        });

        toggle.SetIsOnWithoutNotify(services[activatedIndex].Started);
        toggle.onValueChanged.AddListener(b => { robot.SetServicePaused<T>(!b); });
    }
}