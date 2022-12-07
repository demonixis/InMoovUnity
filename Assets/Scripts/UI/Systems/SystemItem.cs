using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Demonixis.InMoov.UI
{
    public class SystemItem : MonoBehaviour
    {
        private RobotSystem _system;

        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private Toggle _activation;

        private void Start()
        {
            _activation.onValueChanged.AddListener(b =>
            {
                _system.enabled = b;
            });
        }
        
        public void Setup(RobotSystem system)
        {
            _name.text = system.GetType().Name;
            _activation.SetIsOnWithoutNotify(system.enabled);
            _system = system;
        }
    }
}