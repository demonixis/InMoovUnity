using Demonixis.InMoovSharp.Systems;
using UnityEngine;
using static Demonixis.InMoovSharp.Systems.RandomAnimation;

namespace Demonixis.InMoovUnity.Systems
{
    public class RandomAnimationProxy : RobotSystemProxy
    {
        private RandomAnimation _randomAnimation;

        [SerializeField] private ServoAnimation[] _servoActions;
        [SerializeField] private bool _randomHandGestures = true;
        [SerializeField] private float[] _handGestureFrequency = new[] { 1.5f, 6.5f };
        [SerializeField] private bool _randomArmGestures = true;
        [SerializeField] private float[] _armGestureFrequency = new[] { 2.5f, 6.5f };

        protected override void Initialize(UnityRobot robot)
        {
            if (!robot.NativeRobot.TryGetSystem(out _randomAnimation))
            {
                Debug.LogWarning($"Wasn't able to find the RandomAnimation system");
                enabled = false;
            }
            else
                UpdateValues();
        }

        protected override void UpdateValues()
        {
            if (_randomAnimation == null)
                return;

            _randomAnimation.ServoActions = _servoActions;
            _randomAnimation.RandomHandGestures = _randomHandGestures;
            _randomAnimation.HandGestureFrequency = _handGestureFrequency;
            _randomAnimation.RandomArmGestures = _randomArmGestures;
            _randomAnimation.ArmGestureFrequency = _armGestureFrequency;
        }

        [ContextMenu("Populate Servo Actions")]
        public void PopulateServoActions()
        {
            if (_randomAnimation == null) return;
            _servoActions = _randomAnimation.SetupDefaultValues();
        }
    }
}
