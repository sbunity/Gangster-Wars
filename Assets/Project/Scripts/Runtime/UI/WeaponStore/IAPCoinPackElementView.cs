using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SBabchuk.Runtime.UI.WeaponStore
{
    public sealed class IAPCoinPackElementView : IAPProductElementView
    {
        [SerializeField] private Text _amountText;

        private IAssetProvider _assetProvider;

        [Inject]
        public void ConstructRewards(IAssetProvider assetProvider)
        {
            _assetProvider = assetProvider;
        }

        protected override void RefreshContent()
            => _amountText.text = GetCoins().ToString();

        private int GetCoins()
            => _assetProvider.IAPRewardsConfig.TryGetReward(Product, out var reward) ? reward.Coins : 0;
    }
}
