using System;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Services.Contracts;
using Zenject;

namespace SBabchuk.Runtime.Services
{
    public sealed class NoAdsService : INoAdsService, IInitializable, IDisposable
    {
        private readonly IAdsService _adsService;
        private readonly SignalBus _signalBus;

        public NoAdsService(IAdsService adsService, SignalBus signalBus)
        {
            _adsService = adsService;
            _signalBus = signalBus;
        }

        public bool AdsRemoved => !_adsService.IsForcedAdEnabled();

        public void Initialize()
            => _adsService.ForcedAdDisabled += OnForcedAdDisabled;

        public void Dispose()
            => _adsService.ForcedAdDisabled -= OnForcedAdDisabled;

        public void RemoveAds()
            => _adsService.DisableForcedAdForever();

        private void OnForcedAdDisabled()
            => _signalBus.Fire<AdsRemovedSignal>();
    }
}
