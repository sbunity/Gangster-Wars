using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TabController : MonoBehaviour
{
    [SerializeField, FormerlySerializedAs("active")]
    private bool _active;

    [SerializeField, FormerlySerializedAs("imgs")]
    private List<SpriteSwap> _imgs;

    [SerializeField, FormerlySerializedAs("window")]
    private GameObject _window;

    private int _id;

    private void Start()
    {
        _id = GetInstanceID();
        Change(_active);
    }

    public void OnSetFocus(int id)
    {
        _active = id == _id;
        _window.SetActive(_active);
        Change(_active);
    }

    public void SetFocus()
    {
        var parent = transform.parent;
        var tabs = parent != null ? parent.GetComponentsInChildren<TabController>(true) : new[]
        {
            this
        };
        
        foreach (var tab in tabs)
            tab.OnSetFocus(_id);
    }

    public void Change(bool value = true)
    {
        foreach (var spriteSwap in _imgs)
            spriteSwap.Change(value);
    }
}
