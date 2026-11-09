using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using Zenject;

namespace SBabchuk.Runtime.UI
{
    public sealed class LevelCoinsRewardView : CoinCounterView
    {
        [SerializeField, Min(0f)] private float _countUpDelay = 0.4f;

        private ILevelCoinsService _levelCoins;

        protected override bool IgnoreTimeScale => true;

        [Inject]
        public void Construct(ILevelCoinsService levelCoins)
        {
            _levelCoins = levelCoins;
        }

        public void Play()
        {
            SetImmediate(0);
            if (_levelCoins.Earned > 0)
                AnimateTo(_levelCoins.Earned, _countUpDelay);
        }

        protected override string Format(int value)
            => value > 0 ? $"+{value}" : value.ToString();
    }
}
