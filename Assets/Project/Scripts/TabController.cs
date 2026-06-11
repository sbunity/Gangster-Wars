using System.Collections.Generic;
using UnityEngine;

public class TabController : MonoBehaviour
{
    private int _id;

    [Header("Ознака активності вкладки")]
    public bool active;

    [Header("Компоненти, які змінюють вигляд")]
    public List<SpriteSwap> imgs;

    [Header("Вікно, що відповідає закладці")]
    public GameObject window;

    public void Start()
    {
        _id = GetInstanceID();
        Change(active);
    }

    public void OnSetFocus(int id)
    {
        active = id == _id;
        window.SetActive(active);
        Change(active);
    }

    public void SetFocus()
    {
        var parent = transform.parent;
        var tabs = parent != null
            ? parent.GetComponentsInChildren<TabController>(true)
            : new[] { this };

        foreach (var tab in tabs)
            tab.OnSetFocus(_id);
    }

    public void Change(bool value = true)
    {
        foreach (var spriteSwap in imgs)
            spriteSwap.Change(value);
    }
}
