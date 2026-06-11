using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;

public class SpriteSwap : MonoBehaviour
{
    [SerializeField, FormerlySerializedAs("active")]
    private Sprite _active;

    [SerializeField, FormerlySerializedAs("inactive")]
    private Sprite _inactive;

    private Image _img;

    private void Awake()
    {
        _img = GetComponent<Image>();
    }

    public void Change(bool _value = true)
    {
        _img.sprite = _value ? _active : _inactive;
    }
}
