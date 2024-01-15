using System.Collections;
using UnityEngine;

namespace Demonixis.InMoov.Utils
{
    public static class CoroutineFactory
    {
        public static IEnumerator WaitForSecondsUnscaled(float time)
        {
            var start = Time.realtimeSinceStartup;
            while (Time.realtimeSinceStartup < start + time)
            {
                yield return null;
            }
        }

        public static IEnumerator WaitForSeconds(float time)
        {
            var start = Time.realtimeSinceStartup;
            while (Time.realtimeSinceStartup * Time.timeScale < start + time)
            {
                yield return null;
            }
        }
    }
}