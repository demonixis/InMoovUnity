using Demonixis.InMoov;
using Demonixis.InMoov.Chatbots;
using Demonixis.InMoov.ComputerVision;
using Demonixis.InMoov.Navigation;
using Demonixis.InMoov.Services.Speech;
using Demonixis.InMoov.Servos;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class ServicePanel : MonoBehaviour
{
    [Header("Brain")]
    [SerializeField] private TMP_Dropdown _botServiceList;
    [SerializeField] private Toggle _botServiceStatus;
    [SerializeField] private TMP_Dropdown _speechRecognitionServiceList;
    [SerializeField] private Toggle _speechRecognitionServiceStatus;
    [SerializeField] private TMP_Dropdown _speechSynthesisServiceList;
    [SerializeField] private TMP_Dropdown _speechSynthesisVoiceList;
    [SerializeField] private Toggle _speechSynthesisServiceStatus;
    
    [Header("Body")]
    [SerializeField] private TMP_Dropdown _servoMixerServiceList;
    [SerializeField] private Toggle _servoMixerServiceStatus;
    [SerializeField] private TMP_Dropdown _navigationServiceList;
    [SerializeField] private Toggle _navigationServiceStatus;
    
    [Header("Perception")]
    [SerializeField] private TMP_Dropdown _computerVisionServiceList;
    [SerializeField] private Toggle _computerVisionServiceStatus;

    private void Start()
    {
        SetupService<ChatbotService>(_botServiceList, _botServiceStatus);
        SetupService<SpeechSynthesisService>(_speechSynthesisServiceList, _speechSynthesisServiceStatus);
        SetupService<VoiceRecognitionService>(_speechRecognitionServiceList, _speechRecognitionServiceStatus);
        SetupService<NavigationService>(_navigationServiceList, _navigationServiceStatus);
        SetupService<ComputerVisionService>(_computerVisionServiceList, _computerVisionServiceStatus);
        SetupService<ServoMixerService>(_servoMixerServiceList, _servoMixerServiceStatus);
    }

    private void SetupService<T>(TMP_Dropdown dropdown, Toggle toggle) where T : RobotService
    {
        var services = FindObjectsOfType<T>(true);
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
            Robot.Instance.ReplaceService<T>(serviceName);
        });

        toggle.SetIsOnWithoutNotify(services[activatedIndex].Started);
        toggle.onValueChanged.AddListener(b => { Robot.Instance.SetServicePaused<T>(!b); });
    }
}