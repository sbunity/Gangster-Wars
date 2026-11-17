using Cysharp.Threading.Tasks;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine.Scripting;

namespace SBabchuk.Runtime.Advertisement
{
    [Preserve]
    public sealed class AdDummyHandler : AdProviderHandler
    {
        private const float DummyBannerHeight = 50;

        private AdDummyController _dummyController;

        private bool _isInterstitialLoaded;
        private bool _isRewardVideoLoaded;

        public AdDummyHandler(IAdsService adsService, IAdProviderCallbacks callbacks, IServiceLogger logger)
            : base(adsService, callbacks, logger)
        {
        }

        public override string ProviderName => "Dummy";

        protected override UniTask<bool> InitProviderAsync()
        {
            _logger.Log("[AdsManager/Dummy]: InitProviderAsync called");

            _dummyController = AdDummyController.CreateObject();
            _dummyController.Init(_adsSettings, ProviderName, _callbacks);

            return UniTask.FromResult(true);
        }

        public override void ShowBanner()
        {
            _logger.Log($"[AdsManager/Dummy]: ShowBanner called (controller={(_dummyController != null ? "OK" : "NULL")})");

            if (_dummyController == null) return;

            _dummyController.ShowBanner();

            _callbacks.OnProviderAdDisplayed(ProviderName, AdType.Banner);

            _adsService.SetBannerHeight(DummyBannerHeight);
        }

        public override void HideBanner()
        {
            _logger.Log($"[AdsManager/Dummy]: HideBanner called (controller={(_dummyController != null ? "OK" : "NULL")})");

            if (_dummyController == null) return;

            _dummyController.HideBanner();

            _callbacks.OnProviderAdClosed(ProviderName, AdType.Banner);
        }

        public override void DestroyBanner()
        {
            _logger.Log($"[AdsManager/Dummy]: DestroyBanner called (controller={(_dummyController != null ? "OK" : "NULL")})");

            if (_dummyController == null) return;

            _dummyController.HideBanner();

            _callbacks.OnProviderAdClosed(ProviderName, AdType.Banner);
        }

        public override void RequestInterstitial()
        {
            _logger.Log("[AdsManager/Dummy]: RequestInterstitial called");

            _isInterstitialLoaded = true;

            _callbacks.OnProviderAdLoaded(ProviderName, AdType.Interstitial);
        }

        public override bool IsInterstitialLoaded()
        {
            return _isInterstitialLoaded;
        }

        public override void ShowInterstitial(AdvertisementCallback callback)
        {
            _logger.Log($"[AdsManager/Dummy]: ShowInterstitial called (controller={(_dummyController != null ? "OK" : "NULL")}, loaded={_isInterstitialLoaded})");

            if (_dummyController == null) return;

            _dummyController.ShowInterstitial();

            _callbacks.OnProviderAdDisplayed(ProviderName, AdType.Interstitial);
        }

        public override void RequestRewardedVideo()
        {
            _logger.Log("[AdsManager/Dummy]: RequestRewardedVideo called");

            _isRewardVideoLoaded = true;

            _callbacks.OnProviderAdLoaded(ProviderName, AdType.RewardedVideo);
        }

        public override bool IsRewardedVideoLoaded()
        {
            return _isRewardVideoLoaded;
        }

        public override void ShowRewardedVideo(AdvertisementCallback callback)
        {
            _logger.Log($"[AdsManager/Dummy]: ShowRewardedVideo called (controller={(_dummyController != null ? "OK" : "NULL")}, loaded={_isRewardVideoLoaded})");

            if (_dummyController == null) return;

            _dummyController.ShowRewardedVideo();

            _callbacks.OnProviderAdDisplayed(ProviderName, AdType.RewardedVideo);
        }
    }
}
