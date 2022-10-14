using System;
using System.IO.Ports;
using Demonixis.InMoov.Servos;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Demonixis.InMoov.UI
{
    public class ArduinoPanel : MonoBehaviour
    {
        private SerialPortManager _serialPort;

        [SerializeField] private TextMeshProUGUI _status;
        [SerializeField] private TMP_Dropdown _portList;
        [SerializeField] private TMP_Dropdown _cardList;
        [SerializeField] private Button _connectButton;
        [SerializeField] private Button _disconnectedButton;
        
        private void Start()
        {
            _serialPort = FindObjectOfType<SerialPortManager>();
            _cardList.options.Clear();

            var names = Enum.GetNames(typeof(ArduinoIdentifiers));
            foreach (var id in names)
                _cardList.options.Add(new TMP_Dropdown.OptionData(id));

            _cardList.SetValueWithoutNotify(0);
            _cardList.RefreshShownValue();
            _cardList.onValueChanged.AddListener(i => RefreshStatus());
            
            RefreshPorts();
            RefreshStatus();
        }

        public void RefreshStatus()
        {
            var cardId = _cardList.value;
            var connected = _serialPort.IsConnected(cardId);
            _status.text = connected ? "Connected" : "Disconnected";
            _connectButton.interactable = !connected;
            _disconnectedButton.interactable = connected;
        }

        public void RefreshPorts()
        {
            var ports = SerialPort.GetPortNames();

            _portList.options.Clear();

            foreach (var port in ports)
                _portList.options.Add(new TMP_Dropdown.OptionData($"{port}"));

            _portList.SetValueWithoutNotify(0);
            _portList.RefreshShownValue();
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
        }

        public void Disconnect()
        {
            var cardId = _cardList.value;
            _serialPort.Disconnect(cardId);
        }
    }
}