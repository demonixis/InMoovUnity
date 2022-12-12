using Demonixis.InMoov.Servos;
using System;
using System.Collections;
using System.IO.Ports;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Demonixis.InMoov.UI
{
    public sealed class ArduinoPanel : MonoBehaviour
    {
        private SerialPortManager _serialPort;

        [SerializeField] private TextMeshProUGUI _status;
        [SerializeField] private TMP_Dropdown _portList;
        [SerializeField] private TMP_Dropdown _cardList;
        [SerializeField] private Button _connectButton;
        [SerializeField] private Button _disconnectedButton;

        private void Start()
        {
            Robot.Instance.WhenStarted(Initialize);
        }

        private void Initialize()
        {
            _serialPort = FindObjectOfType<SerialPortManager>();
            _cardList.options.Clear();

            var names = Enum.GetNames(typeof(ArduinoIdentifiers));
            foreach (var id in names)
                _cardList.options.Add(new TMP_Dropdown.OptionData(id));

            _cardList.SetValueWithoutNotify(0);
            _cardList.RefreshShownValue();
            _cardList.onValueChanged.AddListener(i => RefreshCardStatus());

            RefreshPorts(true);
            RefreshCardStatus();
            StartCoroutine(RefreshPortsCoroutine());
        }

        private void OnEnable()
        {
            if (_serialPort == null) return;
            StartCoroutine(RefreshPortsCoroutine());
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        public void RefreshCardStatus()
        {
            var cardId = _cardList.value;
            var connected = _serialPort.IsConnected(cardId);
            _status.text = connected ? "Connected" : "Disconnected";
            _connectButton.interactable = !connected;
            _disconnectedButton.interactable = connected;
        }

        private IEnumerator RefreshCardStatusCoroutine()
        {
            yield return new WaitForSeconds(1.0f);
            RefreshCardStatus();
        }

        public void RefreshPorts(bool force)
        {
            var ports = SerialPort.GetPortNames();

            if (!force && ports.Length == _portList.options.Count)
            {
                var samePortCount = 0;
                for (var i = 0; i < ports.Length; i++)
                {
                    if (ports[i] == _portList.options[i].text)
                        samePortCount++;
                }

                // No changes
                if (samePortCount == ports.Length)
                    return;
            }

            _portList.options.Clear();

            foreach (var port in ports)
                _portList.options.Add(new TMP_Dropdown.OptionData($"{port}"));

            _portList.SetValueWithoutNotify(0);
            _portList.RefreshShownValue();
        }

        private IEnumerator RefreshPortsCoroutine()
        {
            var wait = new WaitForSeconds(1.5f);
            while (true)
            {
                RefreshPorts(false);
                yield return wait;
            }
        }

        public void Connect()
        {
            if (_portList.options.Count == 0)
            {
                // TODO: Add a message system and display the info to the user.
                Debug.Log("No port available");
                return;
            }

            var portName = _portList.options[_portList.value].text;
            var cardId = _cardList.value;

            _serialPort.Connect(cardId, portName);

            StartCoroutine(RefreshCardStatusCoroutine());
        }

        public void Disconnect()
        {
            var cardId = _cardList.value;
            _serialPort.Disconnect(cardId);

            StartCoroutine(RefreshCardStatusCoroutine());
        }
    }
}