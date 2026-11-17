using System;
using UnityEngine;
using Zenject;

namespace SBabchuk.Runtime.Advertisement
{
    [Serializable]
    public sealed class AdDummyContainer : AdsProviderContainer
    {
        [SerializeField] private BannerPosition _bannerPosition = BannerPosition.Bottom;
        public BannerPosition BannerPosition => _bannerPosition;

        public override string ProviderName => "Dummy";
        public override AdProviderHandler CreateHandler(IInstantiator instantiator) => instantiator.Instantiate<AdDummyHandler>();
    }
}
