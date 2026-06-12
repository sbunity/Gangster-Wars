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
        }
    }
}
