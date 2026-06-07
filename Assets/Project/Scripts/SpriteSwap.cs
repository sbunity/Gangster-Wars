using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteSwap : MonoBehaviour
{
    [Header("Спрайт, для активного стану")]
    public Sprite active;

    [Header("Спрайт, для неактивного стану")]
    public Sprite inactive;

    [Header("Компонент картинка")]
    private Image img;

    private void Awake()
    {
        img = GetComponent<Image>();
    }

    public void Change(bool _value = true)
    {
        img.sprite = (_value) ? active : inactive;
    }
}
