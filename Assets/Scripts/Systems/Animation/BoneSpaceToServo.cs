using Demonixis.InMoov.Servos;
using UnityEngine;

namespace Demonixis.InMoov.Systems
{
    public sealed class BoneSpaceToServo : MonoBehaviour
    {
        [Header("Servo/Bone Mapping")]
        [SerializeField] private ServoIdentifier _servosYaw = ServoIdentifier.None;
        [SerializeField] private ServoIdentifier _servosPitch = ServoIdentifier.None;
        [SerializeField] private ServoIdentifier _servosRoll = ServoIdentifier.None;
        [SerializeField] private Vector3 _offset;

        [Header("Editor Preview")]
        public Vector3 LocalEuler;
        public Vector3 WorldEuler;
        
        public void ApplyRotation(ServoMixerService service)
        {
            var rotation = transform.localEulerAngles + _offset;

            if (_servosYaw != ServoIdentifier.None)
                service.SetServoValueInEuler(_servosYaw, rotation.y);

            if (_servosPitch != ServoIdentifier.None)
                service.SetServoValueInEuler(_servosPitch, rotation.x);

            if (_servosRoll != ServoIdentifier.None)
                service.SetServoValueInEuler(_servosRoll, rotation.z);
        }
#if UNITY_EDITOR
        private void Update()
        {
            LocalEuler = transform.localEulerAngles + _offset;
            WorldEuler = transform.eulerAngles + _offset;
        }
#endif
    }
}