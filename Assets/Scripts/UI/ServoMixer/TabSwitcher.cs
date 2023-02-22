using UnityEngine;
using UnityEngine.UI;

namespace Demonixis.InMoov.UI
{
    public class TabSwitcher : MonoBehaviour
    {
        [SerializeField] private GameObject[] _tabs;
        [SerializeField] private GameObject[] _buttons;
        [SerializeField] private Color _selectedColor;
        [SerializeField] private Color _unselectedColor;

        private void Start()
        {
            if (_tabs.Length > _buttons.Length)
            {
                Debug.LogError($"Tabs and Buttons array have not the same size!");
                return;
            }

            for (var i = 0; i < _tabs.Length; i++)
                _tabs[i].SetActive(i == 0);

            for (var i = 0; i < _buttons.Length; i++)
                SetTabActive(_buttons[i], i == 0);
        }

        public void SetTabVisible(GameObject target)
        {
            for (var i = 0; i < _tabs.Length; i++)
            {
                var active = _tabs[i] == target;
                SetTabActive(_buttons[i], active);
                _tabs[i].SetActive(active);
            }
        }

        private void SetTabActive(GameObject tab, bool selected)
        {
            var button = tab.GetComponent<Button>();
            var colors = button.colors;
            colors.selectedColor = selected ? _selectedColor : _unselectedColor;
            button.colors = colors;
        }
    }
}