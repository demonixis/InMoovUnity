using System.Collections;
using UnityEngine;

namespace Demonixis.InMoov.Servos
{
    public class ServoMixer : ImService
    {
        private bool _running;
        private ServoData[] _data;

        [SerializeField] private float _updateFrequency = 1.0f / 10.0f;

        public override ImServices Type => ImServices.Servo;

        public override void Initialize()
        {
        }

        public override void SetPaused(bool paused)
        {
            if (!paused)
            {
                StartCoroutine(WriteSerialData());
            }
            else
            {
                _running = false;
            }
        }

        public override void Shutdown()
        {
            _running = false;
        }

        private IEnumerator WriteSerialData()
        {
            var wait = new WaitForSeconds(_updateFrequency);

            _running = true;
            
            while (_running)
            {
                
                
                yield return wait;
            }
        }
    }
}