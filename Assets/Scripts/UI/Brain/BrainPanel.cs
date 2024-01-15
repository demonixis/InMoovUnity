using Demonixis.InMoovSharp.Services;
using Demonixis.InMoovUnity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class BrainPanel : MonoBehaviour
{
    private ChatbotService _chatbot;
    private SpeechSynthesisService _speechSynthesis;
    private VoiceRecognitionService _voiceRecognition;

    [SerializeField] private TMP_InputField _manualBotInput;
    [SerializeField] private TMP_InputField _customSpeechSynthesis;
    [SerializeField] private Transform _botInputContainer;
    [SerializeField] private GameObject _botTextPrefab;
    [SerializeField] private TMP_InputField _wordTrigger;
    [SerializeField] private Image _canListen;
    [SerializeField] private Image _isSpeaking;
    [SerializeField] private TMP_InputField _endSpeakDelay;

    private void Start()
    {
        UnityRobotProxy.Instance.OnRobotReady(Initialize);
    }

    public void Initialize(UnityRobotProxy unityRobot)
    {
        if (_chatbot != null) return;

        var robot = unityRobot.Robot;
        _voiceRecognition = robot.GetService<VoiceRecognitionService>();
        _voiceRecognition.PhraseDetected += s => AppendTextTo(_botInputContainer, s, false);
        _voiceRecognition.ListenChanged += b => _canListen.color = b ? Color.green : Color.red;

        _canListen.color = _voiceRecognition.CanListen ? Color.green : Color.red;
        _isSpeaking.color = Color.gray;

        _wordTrigger.SetTextWithoutNotify(_voiceRecognition.WordTrigger);
        _wordTrigger.onValueChanged.AddListener(s => _voiceRecognition.WordTrigger = s);

        _speechSynthesis = robot.GetService<SpeechSynthesisService>();
        _speechSynthesis.SpeechStarted += m => _isSpeaking.color = Color.green;
        _speechSynthesis.SpeechJustFinished += () => _isSpeaking.color = Color.red;
        _speechSynthesis.SpeechFinishedSafe += () => _isSpeaking.color = Color.gray;

        _endSpeakDelay.SetTextWithoutNotify(_speechSynthesis.DelayAfterSpeak.ToString());
        _endSpeakDelay.onValueChanged.AddListener(s =>
        {
            if (float.TryParse(s, out float result))
                _speechSynthesis.DelayAfterSpeak = result;
            else
                _endSpeakDelay.SetTextWithoutNotify(_speechSynthesis.DelayAfterSpeak.ToString());
        });

        _chatbot = robot.GetService<ChatbotService>();
        _chatbot.ResponseReady += s => AppendTextTo(_botInputContainer, s, true);

        _manualBotInput.onSubmit.AddListener(s =>
        {
            AppendTextTo(_botInputContainer, s, false);
            _chatbot.SubmitResponse(s);
            _manualBotInput.SetTextWithoutNotify(string.Empty);
        });

        _customSpeechSynthesis.onSubmit.AddListener(s => _speechSynthesis.Speak(s));        
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