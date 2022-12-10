using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Demonixis.InMoov
{
    [ExecuteInEditMode]
    public sealed class SliderTextValue : MonoBehaviour
    {
        private TextMeshProUGUI _text;
        private Slider _slider;

        private IEnumerator Start()
        {
            yield return null;

            _text = GetComponent<TextMeshProUGUI>();
            _slider = GetComponentInParent<Slider>();
            _slider.onValueChanged.AddListener(f => { RefreshValue(); });

            RefreshValue();
        }

        public void RefreshValue()
        {
            _text.text = $"{(int) Mathf.Floor(_slider.value)}";
        }
    }
}