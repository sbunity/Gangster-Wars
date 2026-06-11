using System.Collections;
using System.Collections.Generic;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SBabchuk
{
    public class CoinInfo : MonoBehaviour
    {
        [Header("Текстове поле для виведення")]
        private Text txt;
        private IPlayerProgressService _progressService;
        private SignalBus _signalBus;

        [Inject]
        private void Construct(
            IPlayerProgressService progressService,
            SignalBus signalBus)
        {
            _progressService = progressService;
            _signalBus = signalBus;
        }

        private void OnEnable()
        {
            if (_signalBus != null)
                _signalBus.Subscribe<CoinsChangedSignal>(OnCoinsChanged);
        }

        private void OnDisable()
        {
            if (_signalBus != null)
                _signalBus.Unsubscribe<CoinsChangedSignal>(OnCoinsChanged);
        }

        private void Awake()
        {
            txt = GetComponentInChildren<Text>();
        }

        private void Start()
        {
            UpdateCoin();
            
        }

        private void UpdateCoin()
        {
            var coins = _progressService.Coins;

            txt.text = coins.ToString();
        }

        private void OnCoinsChanged(CoinsChangedSignal signal)
        {
            txt.text = signal.Coins.ToString();
        }
    }
}
