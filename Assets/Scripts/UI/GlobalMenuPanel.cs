using UnityEngine;

namespace Demonixis.InMoov.UI
{
    public class GlobalMenuPanel : MonoBehaviour
    {
        private GameObject _currentVisible;
        
        public void SetPanelVisible(GameObject panel)
        {
            if (_currentVisible == panel)
            {
                _currentVisible.SetActive(false);
                return;
            }
            
            if (_currentVisible != null)
                _currentVisible.SetActive(false);

            _currentVisible = panel;
            
            if (_currentVisible != null)
                _currentVisible.SetActive(true);
        }
    }
}