using UnityEngine;

namespace Demonixis.InMoov.UI
{
    public class SystemPanel : MonoBehaviour
    {
        [SerializeField] private GameObject _itemPrefab;
        [SerializeField] private Transform _container;

        private void Start()
        {
            foreach (Transform target in _container)
                Destroy(target.gameObject);
            
            var systems = FindObjectsOfType<RobotSystem>(true);
            foreach (var system in systems)
            {
                var item = Instantiate(_itemPrefab, _container);
                var itemSystem = item.GetComponent<SystemItem>();
                itemSystem.Setup(system);
            }
        }
    }
}