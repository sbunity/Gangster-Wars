using DG.Tweening;
using UnityEngine;

namespace SBabchuk.Runtime.UI
{
    public sealed class UIPulseAnimation : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private float _scaleMultiplier = 1.08f;
        [SerializeField] private float _duration = 0.6f;
        [SerializeField] private Ease _ease = Ease.InOutSine;

        private Vector3 _baseScale = Vector3.one;
        private Tween _tween;
        private bool _playing;

        public bool IsPlaying => _playing;

        private void Awake()
        {
            if (_target == null)
                _target = transform;

            _baseScale = _target.localScale;
        }

        private void OnEnable() => Play();

        private void OnDisable() => Stop();

        public void Play()
        {
            if (_target == null || _playing)
                return;

            _playing = true;
            _tween?.Kill();
            _target.localScale = _baseScale;
            _tween = _target.DOScale(_baseScale * _scaleMultiplier, _duration)
                .SetEase(_ease)
                .SetLoops(-1, LoopType.Yoyo)
                .SetUpdate(true);
        }

        public void Stop()
        {
            if (!_playing)
                return;

            _playing = false;
            _tween?.Kill();
            _tween = null;

            if (_target != null)
                _target.localScale = _baseScale;
        }
    }
}
