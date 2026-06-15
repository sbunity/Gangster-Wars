using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SBabchuk.Runtime.UI
{
    public class CoinInfo : MonoBehaviour
    {
        private Text _txt;
        private CountPulse _countPulse;
        private IPlayerProgressService _progressService;
        private SignalSubscriptions _signals;

        [Inject]
        public void Construct(IPlayerProgressService progressService, SignalBus signalBus)
        {
            _progressService = progressService;
            _signals = new SignalSubscriptions(signalBus)
                .Add<CoinsChangedSignal>(OnCoinsChanged);
        }

        private void OnEnable() => _signals?.Enable();

        private void OnDisable() => _signals?.Disable();

        private void Awake()
        {
            _txt = GetComponentInChildren<Text>();
            _countPulse = GetOrAdd<CountPulse>();
            if (_txt != null)
                _countPulse.SetTarget(_txt.transform);
        }

        private void Start()
        {
            UpdateCoin();
        }

        private void UpdateCoin()
        {
            var coins = _progressService.Coins;
            _txt.text = coins.ToString();
        }

        private void OnCoinsChanged(CoinsChangedSignal signal)
        {
            _txt.text = signal.Coins.ToString();
            if (signal.Delta > 0)
                PlayCollectFeedback();
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
