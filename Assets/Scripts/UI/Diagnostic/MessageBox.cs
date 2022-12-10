using TMPro;
using UnityEngine;

namespace Demonixis.InMoov
{
    public sealed class MessageBox : MonoBehaviour
    {
        private static MessageBox _instance;
        private bool _safeMode;

        [SerializeField] private GameObject _container;
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private TextMeshProUGUI _message;
        [SerializeField] private bool _logToConsole = true;

        public static MessageBox Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<MessageBox>();

                    if (_instance == null)
                    {
                        var go = new GameObject("DebugWindow");
                        _instance = go.AddComponent<MessageBox>();
                        _instance._safeMode = true;
                        _instance._logToConsole = true;
                    }
                }

                return _instance;
            }
        }

        public void Show(string title, string message)
        {
            if (!_safeMode)
            {
                _container.SetActive(true);
                _title.text = title;
                _message.text = message;
            }

            if (_logToConsole)
                Debug.Log(message);
        }

        public void Close()
        {
            if (!_safeMode)
                _container.SetActive(false);
        }
    }
}