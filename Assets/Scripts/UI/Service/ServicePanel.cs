using System;
using System.Collections;
using System.Collections.Generic;
using Demonixis.InMoov;
using Demonixis.InMoov.Chatbots;
using Demonixis.InMoov.ComputerVision;
using Demonixis.InMoov.Navigation;
using Demonixis.InMoov.Services.Speech;
using Demonixis.InMoov.Servos;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ServicePanel : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown _botServiceList;
    [SerializeField] private Toggle _botServiceStatus;
    [SerializeField] private TMP_Dropdown _speechRecognitionServiceList;
    [SerializeField] private Toggle _speechRecognitionServiceStatus;
    [SerializeField] private TMP_Dropdown _speechSynthesisServiceList;
    [SerializeField] private Toggle _speechSynthesisServiceStatus;
    [SerializeField] private TMP_Dropdown _servoMixerServiceList;
    [SerializeField] private Toggle _servoMixerServiceStatus;
    [SerializeField] private TMP_Dropdown _computerVisionServiceList;
    [SerializeField] private Toggle _computerVisionServiceStatus;
    [SerializeField] private TMP_Dropdown _navigationServiceList;
    [SerializeField] private Toggle _navigationServiceStatus;
    
    private void Start()
    {
        SetupDropdown<ChatbotService>(_botServiceList);
        SetupDropdown<SpeechSynthesisService>(_speechSynthesisServiceList);
        SetupDropdown<VoiceRecognitionService>(_speechRecognitionServiceList);
        SetupDropdown<NavigationService>(_navigationServiceList);
        SetupDropdown<ComputerVisionService>(_computerVisionServiceList);
        SetupDropdown<ServoMixerService>(_servoMixerServiceList);
    }

    private void SetupDropdown<T>(TMP_Dropdown dropdown) where T : RobotService
    {
        var services = FindObjectsOfType<T>(true);
        var activatedIndex = 0;
        
        dropdown.options.Clear();
        
        for (var i = 0; i < services.Length; i++)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData(services[i].ServiceName));

            if (services[i].Initialized)
                activatedIndex = i;
        }

        dropdown.SetValueWithoutNotify(activatedIndex);
        dropdown.RefreshShownValue();
    }
}
