using Demonixis.InMoov.Servos;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Demonixis.InMoov.UI
{
    [RequireComponent(typeof(Button))]
    public class ServoMixerItem : MonoBehaviour
    {
        private ServoIdentifier _servoIdentifier;
        public event Action<ServoIdentifier> Clicked;

        public void Setup(ServoIdentifier id)
        {
            _servoIdentifier = id;

            var txt = GetComponentInChildren<TextMeshProUGUI>();
            txt.text = $"{id}";

            var button = GetComponent<Button>();
            button.onClick.AddListener(() => Clicked?.Invoke(_servoIdentifier));
        }
    }
}