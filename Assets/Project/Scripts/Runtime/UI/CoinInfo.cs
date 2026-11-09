using SBabchuk.Runtime.Services.Contracts;
using Zenject;

namespace SBabchuk.Runtime.UI
{
    public class CoinInfo : CoinCounterView
    {
        private ICoinBalance _balance;

        [Inject]
        public void Construct(ICoinBalance balance)
        {
            _balance = balance;
        }

        private void OnEnable()
        {
            if (_balance == null)
                return;

            _balance.Changed += OnCoinsChanged;
            SetImmediate(_balance.Coins);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (_balance != null)
                _balance.Changed -= OnCoinsChanged;
        }

        private void OnCoinsChanged(int delta)
        {
            if (delta > 0)
                AnimateTo(_balance.Coins);
            else
                SetImmediate(_balance.Coins);
        }
    }
}
