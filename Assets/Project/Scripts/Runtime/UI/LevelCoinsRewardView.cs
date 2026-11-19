using System.Threading;
using Cysharp.Threading.Tasks;
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

        public UniTask PlayAsync(CancellationToken cancellationToken)
        {
            SetImmediate(0);
            if (_levelCoins.Earned > 0)
                AnimateTo(_levelCoins.Earned, _countUpDelay);

            return WaitForAnimationAsync(cancellationToken);
        }

        protected override string Format(int value)
            => value > 0 ? $"+{value}" : value.ToString();
    }
}
