using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TabController : MonoBehaviour
{
    public delegate void Focus(int _id);
    public static event Focus OnFocus;

    private int ID;

    [Header("Ознака активності вкладки")]
    public bool active;

    [Header("Компоненти, які змінюють вигляд")]
    public List<SpriteSwap> imgs;

    [Header("Вікно, що відповідає закладці")]
    public GameObject window;

    private void OnEnable()
    {
        TabController.OnFocus += OnSetFocus;
    }

    private void OnDisable()
    {
        TabController.OnFocus -= OnSetFocus;
    }

    public void Start()
    {
        ID = this.GetInstanceID();

        Change(active);
    }

    public void OnSetFocus(int _id)
    {
        active = _id == ID;

        window.SetActive(active);

        Change(active);
    }

    public void SetFocus()
    {
        OnFocus(ID);
    }

    public void Change(bool _value = true)
    {
        foreach (SpriteSwap ss in imgs)
        {
            ss.Change(_value);
        }
    }
}
