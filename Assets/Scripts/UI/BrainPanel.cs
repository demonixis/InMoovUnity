using Demonixis.InMoov;
using System.Collections;
using TMPro;
using UnityEngine;

public class BrainPanel : MonoBehaviour
{
    [SerializeField] private TMP_InputField _manualBotInput;
    [SerializeField] private Transform _botInputContainer;
    [SerializeField] private Transform _botOutputContainer;
    [SerializeField] private GameObject _botTextPrefab;

    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();

        Robot.Instance.Chatbot.ResponseReady += s =>
        {
            AppendTextTo(_botOutputContainer, s);
        };

        _manualBotInput.onSubmit.AddListener(s =>
        {
            Robot.Instance.Chatbot.SubmitResponse(s);
            _manualBotInput.SetTextWithoutNotify(string.Empty);

            AppendTextTo(_botInputContainer, s);
        });
    }

    private void AppendTextTo(Transform parent, string text)
    {
        var go = Instantiate(_botTextPrefab, parent);
        var txt = go.GetComponent<TextMeshProUGUI>();
        txt.text = text;
    }
}
