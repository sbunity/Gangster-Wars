using DG.Tweening;
using UnityEngine;

namespace SBabchuk.Runtime.UI.Popups
{
    [CreateAssetMenu(menuName = "UI/Create UIPopupStyle", fileName = "UIPopupStyle")]
    public sealed class UIPopupStyle : ScriptableObject
    {
        [Header("Show")]
        [SerializeField, Range(0.01f, 1f)] private float _showFromScale = 0.8f;
        [SerializeField, Min(0.01f)] private float _showDuration = 0.3f;
        [SerializeField] private Ease _showEase = Ease.OutBack;
        [SerializeField, Min(0f)] private float _showOvershoot = 1.4f;
        [SerializeField, Min(0.01f)] private float _fadeInDuration = 0.15f;

        [Header("Hide")]
        [SerializeField, Range(0.01f, 1f)] private float _hideToScale = 0.9f;
        [SerializeField, Min(0.01f)] private float _hideDuration = 0.15f;
        [SerializeField] private Ease _hideEase = Ease.InQuad;

        public float ShowFromScale => _showFromScale;
        public float ShowDuration => _showDuration;
        public Ease ShowEase => _showEase;
        public float ShowOvershoot => _showOvershoot;
        public float FadeInDuration => _fadeInDuration;
        public float HideToScale => _hideToScale;
        public float HideDuration => _hideDuration;
        public Ease HideEase => _hideEase;
    }
}
