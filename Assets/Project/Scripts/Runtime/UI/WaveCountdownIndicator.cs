using DG.Tweening;
using SBabchuk.Runtime.Architecture;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SBabchuk.Runtime.UI
{
    public sealed class WaveCountdownIndicator : MonoBehaviour
    {
        [SerializeField] private Image _fill;
        [SerializeField] private float _rewindDuration = 0.3f;
        [SerializeField] private Ease _rewindEase = Ease.OutCubic;

        private SignalSubscriptions _signals;
        private Sequence _sequence;
        private bool _rewindPending;

        [Inject]
        public void Construct(SignalBus signalBus)
        {
            _signals = new SignalSubscriptions(signalBus)
                .Add<WaveCountdownStartedSignal>(OnCountdownStarted)
                .Add<WaveCountdownSkippedSignal>(OnCountdownSkipped);
            _signals.Enable();
        }

        private void Awake()
        {
            if (_fill == null)
                _fill = GetComponent<Image>();

            if (_fill != null)
                _fill.fillAmount = 0f;
        }

        private void OnEnable() => _signals?.Enable();

        private void OnDisable() => _signals?.Disable();

        private void OnDestroy() => _sequence?.Kill();

        private void OnCountdownStarted(WaveCountdownStartedSignal signal)
        {
            if (_fill == null)
                return;

            _sequence?.Kill();
            var sequence = DOTween.Sequence();

            if (_rewindPending && _fill.fillAmount > 0f)
                sequence.Append(Tween(0f, _rewindDuration, _rewindEase));
            else
                _fill.fillAmount = 0f;

            if (signal.Duration > 0f)
                sequence.Append(Tween(1f, signal.Duration, Ease.Linear));
            else
                sequence.AppendCallback(() => _fill.fillAmount = 1f);

            _rewindPending = false;
            _sequence = sequence;
        }

        private void OnCountdownSkipped()
        {
            if (_fill == null)
                return;

            _rewindPending = true;
            _sequence?.Kill();
            _sequence = DOTween.Sequence().Append(Tween(0f, _rewindDuration, _rewindEase));
        }

        private Tween Tween(float endValue, float duration, Ease ease)
            => DOTween.To(() => _fill.fillAmount, value => _fill.fillAmount = value, endValue, duration)
                .SetEase(ease);
    }
}
