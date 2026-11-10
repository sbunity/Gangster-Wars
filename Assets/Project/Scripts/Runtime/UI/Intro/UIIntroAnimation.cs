using DG.Tweening;
using UnityEngine;

namespace SBabchuk.Runtime.UI.Intro
{
    public abstract class UIIntroAnimation : MonoBehaviour
    {
        [SerializeField] private UIIntroStyle _styleOverride;
        [SerializeField, Min(0f)] private float _extraDelay;

        public float ExtraDelay => _extraDelay;

        public void Hide(UIIntroStyle defaultStyle) => ApplyHidden(StyleOr(defaultStyle));

        public Tween Play(UIIntroStyle defaultStyle) => CreateShowTween(StyleOr(defaultStyle));

        protected abstract void ApplyHidden(UIIntroStyle style);

        protected abstract Tween CreateShowTween(UIIntroStyle style);

        private UIIntroStyle StyleOr(UIIntroStyle defaultStyle)
            => _styleOverride != null ? _styleOverride : defaultStyle;
    }
}
