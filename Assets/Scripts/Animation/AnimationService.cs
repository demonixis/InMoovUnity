using System.Collections;
using Demonixis.InMoov;
using UnityEngine;

public class AnimationService : RobotService
{
    private bool _running;
    
    [Header("Config")] [SerializeField] private float _updateInterval = 1.0f / 30.0f;
    
    [Header("Rig")]
    [SerializeField] private Transform _neck;

    public override RobotServices Type { get; } = RobotServices.Other;
    
    public override void Initialize()
    {
        StartCoroutine(Loop());
    }

    private IEnumerator Loop()
    {
        _running = true;

        var interval = new WaitForSeconds(_updateInterval);

        while (_running)
        {
            var neckRotation = ClampedVector0to180(_neck.rotation.eulerAngles);
           

            yield return interval;
        }
    }

    private static Vector3 ClampedVector0to180(Vector3 target)
    {
        target.x = (target.x % 360.0f) / 2.0f;
        target.y = (target.y % 360.0f) / 2.0f;
        target.z = (target.z % 360.0f) / 2.0f;
        return target;
    }

    public override void SetPaused(bool paused)
    {
        _running = !paused;

        if (!paused)
            StartCoroutine(Loop());
    }

    public override void Shutdown()
    {
        _running = false;
    }
}
