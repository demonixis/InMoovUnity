using Demonixis.InMoovSharp.Systems;
using UnityEngine;

namespace Demonixis.InMoovUnity.Systems
{
    public class JawMechanismProxy : RobotSystemProxy
    {
        private JawMechanism _jawMechanism;

        [SerializeField] private float _jawOpenTime = 0.25f;
        [SerializeField] private float _jawCloseTime = 0.15f;
        [SerializeField] private byte _jawAmplitude = 40;
        [SerializeField] private byte _jawNeutralOffset = 10;

        protected override void Initialize(UnityRobot robot)
        {
            if (!robot.NativeRobot.TryGetSystem(out _jawMechanism))
            {
                Debug.LogWarning($"Wasn't able to find the JawMechanism system");
                enabled = false;
            }
            else
                UpdateValues();
        }

        protected override void UpdateValues()
        {
            if (_jawMechanism == null) 
                return;
            
            _jawMechanism.JawOpenTime = _jawOpenTime;
            _jawMechanism.JawCloseTime = _jawCloseTime;
            _jawMechanism.JawAmplitude = _jawAmplitude;
            _jawMechanism.JawNeutralOffset = _jawNeutralOffset;
        }
    }
}
