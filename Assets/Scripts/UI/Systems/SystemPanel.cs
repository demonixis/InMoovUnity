using Demonixis.InMoovUnity;
using UnityEngine;

namespace Demonixis.InMoov.UI
{
    public sealed class SystemPanel : MonoBehaviour
    {
        [SerializeField] private GameObject _itemPrefab;
        [SerializeField] private Transform _container;

        private void Start()
        {
            UnityRobotProxy.Instance.OnRobotReady(Initialize);
        }

        private void Initialize(UnityRobotProxy unityRobot)
        {
            foreach (Transform target in _container)
                Destroy(target.gameObject);

            var robot = unityRobot.Robot;

            var systems = robot.Systems;
            foreach (var system in systems)
            {
                var item = Instantiate(_itemPrefab, _container);
                var itemSystem = item.GetComponent<SystemItem>();
                itemSystem.Setup(system);
            }
        }
    }
}