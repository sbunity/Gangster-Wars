using DG.Tweening;
using UnityEngine;

namespace SBabchuk.Runtime.UI
{
    public sealed class CountPulse : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField, Range(0.05f, 0.5f)] private float _duration = 0.24f;
        [SerializeField, Range(0.01f, 0.5f)] private float _strength = 0.16f;
        [SerializeField, Range(1, 8)] private int _vibrato = 3;

        private Tween _tween;
        private Vector3 _baseScale;

        private Transform Target => _target != null ? _target : transform;

        public void SetTarget(Transform target)
        {
            _target = target;
            _baseScale = Target.localScale;
        }

        private void Awake()
        {
            _baseScale = Target.localScale;
        }

        private void OnEnable()
        {
            _baseScale = Target.localScale;
        }

        private void OnDisable()
        {
            _tween?.Kill();
            Target.localScale = _baseScale;
        }

        public void Play()
        {
            _tween?.Kill();
            Target.localScale = _baseScale;
            _tween = Target
                .DOPunchScale(Vector3.one * _strength, _duration, _vibrato, 0.65f)
                .SetEase(Ease.OutQuad)
                .OnKill(() => Target.localScale = _baseScale);
        }
    }
}
