using Demonixis.InMoov.Servos;
using Demonixis.InMoov.Utils;
using UnityEngine;

namespace Demonixis.InMoov.Systems
{
    public class MecanimToServo : RobotSystem
    {
        private ServoMixerService _servoMixer;
        private BoneSpaceToServo[] _bones;

        [SerializeField] GameObject _rig;
        
        public override void Initialize()
        {
            _servoMixer =  Robot.Instance.GetService<ServoMixerService>();
            _bones = _rig.GetComponentsInChildren<BoneSpaceToServo>();
            base.Initialize();
        }

        private void LateUpdate()
        {
            if (!Started) return;
            foreach (var bone in _bones)
                bone.ApplyRotation(_servoMixer);
        }
    }
}