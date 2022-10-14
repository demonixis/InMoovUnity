using System;
using Demonixis.InMoov.Servos;
using UnityEngine;

namespace Demonixis.InMoov.UI
{
    public class ServoMixerPanel : MonoBehaviour
    {
        private ServoMixerService _servoMixerService;
        
        [SerializeField] private Transform _servoList;
        [SerializeField] private GameObject _servoItemPrefab;

        private void Start()
        {
            _servoMixerService = FindObjectOfType<ServoMixerService>();
            
            foreach (Transform child in _servoList)
                Destroy(child.gameObject);

            var names = Enum.GetNames(typeof(ServoIdentifier));

           for(var i = 0; i < names.Length; i++)
            {
                var item = Instantiate(_servoItemPrefab, _servoList);
                ResetTransform(item.transform);

                var servoItem = item.AddComponent<ServoMixerItem>();
                servoItem.Setup((ServoIdentifier)i);
                servoItem.Clicked += OnServoClicked;
            }
        }

        private void OnServoClicked(ServoIdentifier id)
        {
            var data = _servoMixerService.GetServoData(id);
            UpdateServoDetails(id, data);
        }

        private void UpdateServoDetails(ServoIdentifier id, ServoData data)
        {
            
        }

        private void ResetTransform(Transform target)
        {
            target.localPosition = Vector3.zero;
            target.localRotation = Quaternion.identity;
            target.localScale = Vector3.one;
        }
    }
}