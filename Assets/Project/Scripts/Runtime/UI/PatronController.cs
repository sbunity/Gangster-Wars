using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;

namespace SBabchuk.Runtime.UI
{
    public class PatronController : MonoBehaviour
    {
        [SerializeField, FormerlySerializedAs("index")]
        private int _index;

        [SerializeField, FormerlySerializedAs("fullSprite")]
        private Sprite _fullSprite;

        [SerializeField, FormerlySerializedAs("emptySprite")]
        private Sprite _emptySprite;

        private Image _ico;

        public int Index => _index;

        private Image Ico => _ico != null ? _ico : _ico = GetComponentInChildren<Image>(true);

        public void SetFilled(bool filled)
        {
            Ico.sprite = filled ? _fullSprite : _emptySprite;
        }
    }
}
