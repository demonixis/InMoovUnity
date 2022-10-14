using System;
using Demonixis.InMoov.Settings;
using TMPro;
using UnityEngine;

namespace Demonixis.InMoov.UI
{
    public class GlobalSettingsPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown _languagesList;

        private void Start()
        {
            var names = Enum.GetNames(typeof(SupportedLanguages));
            
            _languagesList.options.Clear();
            foreach (var lang in names)
            {
                _languagesList.options.Add(new TMP_Dropdown.OptionData(lang));
            }
            
            _languagesList.SetValueWithoutNotify(0);
            _languagesList.RefreshShownValue();
        }
    }
}