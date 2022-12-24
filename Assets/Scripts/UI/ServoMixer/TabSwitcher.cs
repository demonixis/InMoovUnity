using UnityEngine;

public class TabSwitcher : MonoBehaviour
{ 
    [SerializeField] private GameObject[] _tabs;

    private void Start()
    {
        foreach (var tab in _tabs)
            tab.SetActive(tab == _tabs[0]);
    }

    public void SetTabVisible(GameObject target)
    {
        foreach (var tab in _tabs)
            tab.SetActive(tab == target);
    }
}
