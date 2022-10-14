using System;
using Demonixis.InMoov.Servos;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Demonixis.InMoov.UI
{
    public class ServoMixerPanel : MonoBehaviour
    {
        private ServoMixerService _servoMixerService;
        private ServoIdentifier _currentServo;
        private ServoData _currentData;

        [Header("Servo List"), SerializeField] private Transform _servoList;

        [SerializeField] private GameObject _servoItemPrefab;

        [Header("Servo Settings"), SerializeField]
        private TextMeshProUGUI _servoName;

        [SerializeField] private TMP_Dropdown _servoCardId;
        [SerializeField] private TMP_Dropdown _servoPinId;
        [SerializeField] private Toggle _servoEnabled;
        [SerializeField] private Toggle _servoReversed;
        [SerializeField] private Slider _servoNeutral;
        [SerializeField] private Slider _servoMin;
        [SerializeField] private Slider _servoMax;
        [SerializeField] private Slider _servoSpeed;
        [SerializeField] private Slider _servoValue;

        private void Start()
        {
            _servoMixerService = FindObjectOfType<ServoMixerService>();

            foreach (Transform child in _servoList)
                Destroy(child.gameObject);

            var names = Enum.GetNames(typeof(ServoIdentifier));

            for (var i = 0; i < names.Length; i++)
            {
                var item = Instantiate(_servoItemPrefab, _servoList);
                ResetTransform(item.transform);

                var servoItem = item.AddComponent<ServoMixerItem>();
                servoItem.Setup((ServoIdentifier) i);
                servoItem.Clicked += OnServoClicked;
            }

            names = Enum.GetNames(typeof(ArduinoIdentifiers));
            _servoCardId.options.Clear();
            foreach (var item in names)
                _servoCardId.options.Add(new TMP_Dropdown.OptionData(item));

            _servoPinId.options.Clear();
            for (var i = SerialPortManager.PinStart; i <= SerialPortManager.PinEnd; i++)
                _servoPinId.options.Add(new TMP_Dropdown.OptionData($"Pin #{i}"));
            
            // Bind events
            _servoPinId.onValueChanged.AddListener(i =>
            {
                _currentData.PinId = _servoPinId.value + 2;
                UpdateDataOnArduino();
            });
            
            _servoCardId.onValueChanged.AddListener(i =>
            {
                _currentData.CardId = i;
                UpdateDataOnArduino();
            });
            
            _servoEnabled.onValueChanged.AddListener(b =>
            {
                _currentData.Enabled = b;
                UpdateDataOnArduino();
            });

            _servoNeutral.minValue = 0;
            _servoNeutral.maxValue = 180;
            _servoNeutral.value = 90;
            _servoNeutral.onValueChanged.AddListener(i =>
            {
                _currentData.Neutral = (int)i;
                UpdateDataOnArduino();
            });
            
            _servoMin.minValue = 0;
            _servoMin.maxValue = 180;
            _servoMin.value = 90;
            _servoMin.onValueChanged.AddListener(i =>
            {
                _currentData.Min = (int)i;
                UpdateDataOnArduino();
            });
            
            _servoMax.minValue = 0;
            _servoMax.maxValue = 180;
            _servoMax.value = 90;
            _servoMax.onValueChanged.AddListener(i =>
            {
                _currentData.Max = (int)i;
                UpdateDataOnArduino();
            });
            
            _servoSpeed.minValue = 0;
            _servoSpeed.maxValue = 1;
            _servoSpeed.value = 1;
            _servoSpeed.onValueChanged.AddListener(i =>
            {
                _currentData.Speed = (int)i;
                UpdateDataOnArduino();
            });
        }

        private void UpdateDataOnArduino()
        {
            _servoMixerService.SetServoData(_currentServo, _currentData);
        }

        private void OnServoClicked(ServoIdentifier id)
        {
            var data = _servoMixerService.GetServoData(id);
            UpdateServoDetails(id, data);
        }

        private void UpdateServoDetails(ServoIdentifier id, ServoData data)
        {
            _currentServo = id;
            _currentData = data;

            _servoName.text = $"{id}";
            _servoCardId.SetValueWithoutNotify(data.CardId);
            _servoEnabled.SetIsOnWithoutNotify(data.Enabled);
            _servoReversed.SetIsOnWithoutNotify(data.Inverse);
            _servoNeutral.SetValueWithoutNotify(data.Neutral);
            _servoMin.SetValueWithoutNotify(data.Min);
            _servoMax.SetValueWithoutNotify(data.Max);
            _servoSpeed.SetValueWithoutNotify(data.Speed);

            var pinValue = FindPinValueFromPinId(data.PinId);
            _servoPinId.SetValueWithoutNotify(pinValue);
            _servoPinId.RefreshShownValue();
            
            _servoCardId.SetValueWithoutNotify(data.CardId);
            _servoCardId.RefreshShownValue();
        }

        private int FindPinValueFromPinId(int pinId)
        {
            for (var i = 0; i < _servoPinId.options.Count; i++)
            {
                var tmp = _servoPinId.options[i].text.Split('#');
                if (tmp.Length != 2)
                {
                    Debug.LogException(new UnityException($"The item {_servoPinId.options[i]} hasn't a valid name."));
                }

                var value = int.Parse(tmp[1]);
                if (value == pinId)
                    return i;
            }

            return 0;
        }

        private void ResetTransform(Transform target)
        {
            target.localPosition = Vector3.zero;
            target.localRotation = Quaternion.identity;
            target.localScale = Vector3.one;
        }
    }
}