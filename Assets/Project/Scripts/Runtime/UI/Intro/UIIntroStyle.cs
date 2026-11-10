using DG.Tweening;
using UnityEngine;

namespace SBabchuk.Runtime.UI.Intro
{
    [CreateAssetMenu(menuName = "UI/Create UIIntroStyle", fileName = "UIIntroStyle")]
    public sealed class UIIntroStyle : ScriptableObject
    {
        [SerializeField, Min(0.01f)] private float _duration = 0.4f;
        [SerializeField] private Ease _ease = Ease.OutBack;
        [SerializeField, Min(0f)] private float _overshoot = 1.2f;
        [SerializeField, Min(0f)] private float _offscreenMargin = 30f;

        public float Duration => _duration;
        public Ease Ease => _ease;
        public float Overshoot => _overshoot;
        public float OffscreenMargin => _offscreenMargin;
    }
}
