using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SBabchuk
{
    public class CoinInfo : MonoBehaviour
    {
        private Text _txt;
        private IPlayerProgressService _progressService;
        private SignalBus _signalBus;

        [Inject]
        public void Construct(IPlayerProgressService progressService, SignalBus signalBus)
        {
            _progressService = progressService;
            _signalBus = signalBus;
        }

        private void OnEnable()
        {
            _signalBus?.Subscribe<CoinsChangedSignal>(OnCoinsChanged);
        }

        private void OnDisable()
        {
            _signalBus?.Unsubscribe<CoinsChangedSignal>(OnCoinsChanged);
        }

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
