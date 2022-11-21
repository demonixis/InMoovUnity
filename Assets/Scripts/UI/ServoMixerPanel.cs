using System;
using Demonixis.InMoov.Servos;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
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
            _currentData = ServoData.New(string.Empty);
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
            _servoPinId.value = _currentData.PinId;
            _servoPinId.RefreshShownValue();
            _servoPinId.onValueChanged.AddListener(i =>
            {
                _currentData.PinId = (byte)(_servoPinId.value + SerialPortManager.PinStart);
                UpdateDataOnArduino();
            });

            _servoCardId.value = _currentData.CardId;
            _servoCardId.RefreshShownValue();
            _servoCardId.onValueChanged.AddListener(i =>
            {
                _currentData.CardId = i;
                UpdateDataOnArduino();
            });

            _servoEnabled.isOn = _currentData.Enabled > 0;
            _servoEnabled.onValueChanged.AddListener(b =>
            {
                _currentData.Enabled = (byte)(b ? 1 : 0);
                UpdateDataOnArduino();
            });

            SetupSlider(_servoNeutral, 0, 180, _currentData.Neutral, i =>
            {
                _currentData.Neutral = (byte)i;
                UpdateDataOnArduino();
            });

            SetupSlider(_servoMin, 0, 180, _currentData.Min, i =>
            {
                _currentData.Min = (byte)i;
                UpdateDataOnArduino();
            });

            SetupSlider(_servoMax, 0, 180, _currentData.Max, i =>
            {
                _currentData.Max = (byte)i;
                UpdateDataOnArduino();
            });

            SetupSlider(_servoSpeed, 10, 100, _currentData.Speed, i =>
            {
                _currentData.Speed = (byte)i;
                UpdateDataOnArduino();
            });

            SetupSlider(_servoValue, 0, 180, _currentData.Value, i =>
            {
                _currentData.Value = (byte)i;
                UpdateDataOnArduino();
            });
        }

        private void SetupSlider(Slider slider, float min, float max, float value, UnityAction<float> onValueChanged)
        {
            slider.minValue = 0;
            slider.maxValue = 180;
            slider.value = 90;
            _servoValue.onValueChanged.AddListener(onValueChanged);
        }

        private void UpdateDataOnArduino()
        {
            _servoMixerService.SetServoData(_currentServo, ref _currentData);
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
            _servoEnabled.SetIsOnWithoutNotify(data.Enabled > 0);
            _servoReversed.SetIsOnWithoutNotify(data.Inverse);
            _servoNeutral.SetValueWithoutNotify(data.Neutral);
            _servoMin.SetValueWithoutNotify(data.Min);
            _servoMax.SetValueWithoutNotify(data.Max);
            _servoSpeed.SetValueWithoutNotify(data.Speed);
            _servoValue.SetValueWithoutNotify(data.Value);

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