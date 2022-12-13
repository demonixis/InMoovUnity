using Demonixis.InMoov;
using Demonixis.InMoov.Chatbots;
using Demonixis.InMoov.Services.Speech;
using TMPro;
using UnityEngine;

public sealed class BrainPanel : MonoBehaviour
{
    private ChatbotService _chatbot;

    [SerializeField] private TMP_InputField _manualBotInput;
    [SerializeField] private TMP_InputField _customSpeechSynthesis;
    [SerializeField] private Transform _botInputContainer;
    [SerializeField] private GameObject _botTextPrefab;

    private void Start()
    {
        Robot.Instance.WhenStarted(Initialize);
    }

    public void Initialize()
    {
        if (_chatbot != null) return;

        var robot = Robot.Instance;
        if (robot.TryGetService(out VoiceRecognitionService voiceRecognitionService))
        {
            voiceRecognitionService.PhraseDetected += s => AppendTextTo(_botInputContainer, s, false);
        }

        _chatbot = robot.GetService<ChatbotService>();
        _chatbot.ResponseReady += s => AppendTextTo(_botInputContainer, s, true);

        _manualBotInput.onSubmit.AddListener(s =>
        {
            AppendTextTo(_botInputContainer, s, false);
            _chatbot.SubmitResponse(s);
            _manualBotInput.SetTextWithoutNotify(string.Empty);
        });
        
        _customSpeechSynthesis.onSubmit.AddListener(s =>
        {
            var speech = Robot.Instance.GetService<SpeechSynthesisService>();
            speech.Speak(s);
        });
    }

    private void AppendTextTo(Transform parent, string text, bool justifyRight)
    {
        var go = Instantiate(_botTextPrefab, parent);
        var txt = go.GetComponent<TextMeshProUGUI>();
        txt.text = text;
        txt.alignment = justifyRight ? TextAlignmentOptions.MidlineRight : TextAlignmentOptions.MidlineLeft;
        txt.color = justifyRight ? Color.green : Color.cyan;
    }
}