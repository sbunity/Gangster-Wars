using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;

namespace SBabchuk.Runtime.UI.WeaponStore
{
    public class LoadGrenadeInfoUI : MonoBehaviour
    {
        [SerializeField, FormerlySerializedAs("type")]
        private GrenadesName _type;
        public GrenadesName Type { get => _type; set => _type = value; }

        [SerializeField, FormerlySerializedAs("ico")]
        private Image _icon;

        [SerializeField, FormerlySerializedAs("count")]
        private Text _count;

        public void Initialized(Sprite icon, int count)
        {
            _count.text = count.ToString();
            _icon.sprite = icon;
        }
    }
}
