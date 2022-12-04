using System;
using TMPro;
using UnityEngine;

namespace Demonixis.InMoov.UI
{
    public class GlobalSettingsPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown _languagesList;

        private void Start()
        {
            var languages = Enum.GetNames(typeof(SystemLanguage));

            _languagesList.options.Clear();

            foreach (var lang in languages)
            {
                _languagesList.options.Add(new TMP_Dropdown.OptionData(lang));
            }

            _languagesList.SetValueWithoutNotify((int)SystemLanguage.English);
            _languagesList.RefreshShownValue();
        }
    }
}