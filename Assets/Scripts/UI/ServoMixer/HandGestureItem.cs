using Demonixis.InMoov.UI;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HandGestureItem : MonoBehaviour
{
    [SerializeField] private HandGestures _gesture = HandGestures.Rock;
    
    public event Action<HandGestures> Clicked;

    public void Setup(HandGestures item)
    {
        _gesture = item;

        var text = GetComponentInChildren<TextMeshProUGUI>();
        text.text = item.ToString();

        var button = GetComponent<Button>();
        button.onClick.AddListener(() => Clicked?.Invoke(_gesture));
    }
}
