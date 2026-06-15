using DG.Tweening;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SBabchuk.Runtime.UI
{
    public class CoinInfo : MonoBehaviour
    {
        private const float CountUpDuration = 0.55f;

        private Text _txt;
        private CountPulse _countPulse;
        private IPlayerProgressService _progressService;
        private SignalSubscriptions _signals;
        private Tween _countTween;
        private int _displayed;

        [Inject]
        public void Construct(IPlayerProgressService progressService, SignalBus signalBus)
        {
            _progressService = progressService;
            _signals = new SignalSubscriptions(signalBus)
                .Add<CoinsChangedSignal>(OnCoinsChanged);
        }

        private void OnEnable() => _signals?.Enable();

        private void OnDisable()
        {
            _signals?.Disable();
            _countTween?.Kill();
        }

        private void Awake()
        {
            _txt = GetComponentInChildren<Text>();
            _countPulse = GetOrAdd<CountPulse>();
            if (_txt != null)
                _countPulse.SetTarget(_txt.transform);
        }

        private void Start()
        {
            SetImmediate(_progressService.Coins);
        }

        private void OnCoinsChanged(CoinsChangedSignal signal)
        {
            if (signal.Delta > 0)
                AnimateTo(signal.Coins);
            else
                SetImmediate(signal.Coins);
        }

        private void AnimateTo(int value)
        {
            _countTween?.Kill();
            _countTween = DOTween.To(() => _displayed, SetDisplayed, value, CountUpDuration)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    SetDisplayed(value);
                    PlayCollectFeedback();
                });
        }

        private void SetImmediate(int value)
        {
            _countTween?.Kill();
            SetDisplayed(value);
        }

        private void SetDisplayed(int value)
        {
            _displayed = value;
            if (_txt != null)
                _txt.text = value.ToString();
        }

        private void PlayCollectFeedback()
        {
            _countPulse ??= GetOrAdd<CountPulse>();
            if (_txt != null)
                _countPulse.SetTarget(_txt.transform);
            _countPulse.Play();
        }

        private T GetOrAdd<T>() where T : Component
        {
            var component = GetComponent<T>();
            return component ?? gameObject.AddComponent<T>();
        }
    }
}
