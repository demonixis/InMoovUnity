using Demonixis.InMoovSharp.Systems;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Demonixis.InMoov.UI
{
    public class UIGestureItem : MonoBehaviour
    {
        private int _gesture;

        public event Action<int> Clicked;

        public void SetupHandGesture(int item)
        {
            _gesture = item;
            var gesture = (HandGestures)item;
            BaseSetup(gesture.ToString());
        }

        public void SetupArmGesture(int item)
        {
            _gesture = (int)item;
            var gesture = (ArmGestures)item;
            BaseSetup(gesture.ToString());
        }

        private void BaseSetup(string gesture)
        {
            var text = GetComponentInChildren<TextMeshProUGUI>();
            text.text = gesture;

            var button = GetComponent<Button>();
            button.onClick.AddListener(() => Clicked?.Invoke(_gesture));
        }
    }
}