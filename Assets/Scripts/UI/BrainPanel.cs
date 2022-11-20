using Demonixis.InMoov;
using System.Collections;
using TMPro;
using UnityEngine;

public class BrainPanel : MonoBehaviour
{
    [SerializeField] private TMP_InputField _manualBotInput;
    [SerializeField] private Transform _botInputContainer;
    [SerializeField] private GameObject _botTextPrefab;

    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();

        Robot.Instance.VoiceRecognition.PhraseDetected += s =>
        {
            AppendTextTo(_botInputContainer, s, false);
        };

        Robot.Instance.Chatbot.ResponseReady += s =>
        {
            AppendTextTo(_botInputContainer, s, true);
        };

        _manualBotInput.onSubmit.AddListener(s =>
        {
            AppendTextTo(_botInputContainer, s, false);
            Robot.Instance.Chatbot.SubmitResponse(s);
            _manualBotInput.SetTextWithoutNotify(string.Empty);
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
