using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using Zenject;

namespace SBabchuk.Runtime.UI.WeaponStore
{
    public sealed class NoAdsElementView : IAPProductElementView
    {
        [SerializeField] private GameObject _purchasedMark;

        private INoAdsService _noAdsService;

        [Inject]
        public void ConstructNoAds(INoAdsService noAdsService)
        {
            _noAdsService = noAdsService;
        }

        protected override void SubscribeSignals(SignalSubscriptions signals)
            => signals.Add<AdsRemovedSignal>(Refresh);

        protected override void RefreshContent()
        {
            var adsRemoved = _noAdsService.AdsRemoved;
            BuyButton.gameObject.SetActive(!adsRemoved);
            _purchasedMark.SetActive(adsRemoved);
        }
    }
}
