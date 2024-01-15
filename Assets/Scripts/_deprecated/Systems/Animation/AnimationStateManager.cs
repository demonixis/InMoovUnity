using Demonixis.InMoov.Services.Speech;
using System.Data;

namespace Demonixis.InMoov.Systems
{
    public class AnimationStateManager : RobotSystem
    {
        private AnimationManager _animationManager;
        private AnimationState _previousState;

        public override void Initialize()
        {
            base.Initialize();

            _animationManager = FindObjectOfType<AnimationManager>();

            var robot = Robot.Instance;
            var speechSynthesis = robot.GetService<SpeechSynthesisService>();
            speechSynthesis.SpeechStarted += SpeechSynthesis_SpeechStarted;
            speechSynthesis.SpeechJustFinished += SpeechSynthesis_SpeechJustFinished;

            UpdateState(AnimationState.Idle);
        }

        public override void Dispose()
        {
            var robot = Robot.Instance;
            var speechSynthesis = robot.GetService<SpeechSynthesisService>();
            if (speechSynthesis == null) return;
            speechSynthesis.SpeechStarted -= SpeechSynthesis_SpeechStarted;
            speechSynthesis.SpeechJustFinished -= SpeechSynthesis_SpeechJustFinished;

            UpdateState(AnimationState.None);

            base.Dispose();
        }

        private void SpeechSynthesis_SpeechJustFinished()
        {
            UpdateState(_previousState);
        }

        private void SpeechSynthesis_SpeechStarted(string obj)
        {
            UpdateState(AnimationState.Speak);
        }

        private void UpdateState(AnimationState state)
        {
            _previousState = _animationManager.State;
            _animationManager.State = state;
        }
    }
}
