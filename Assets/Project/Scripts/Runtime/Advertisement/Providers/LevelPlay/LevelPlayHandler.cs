#pragma warning disable 0618

using Cysharp.Threading.Tasks;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;

#if MODULE_LEVELPLAY
using Unity.Services.LevelPlay;
#endif

namespace SBabchuk.Runtime.Advertisement
{
#if MODULE_LEVELPLAY
    [UnityEngine.Scripting.Preserve]
    public sealed class LevelPlayHandler : AdProviderHandler
    {
        private readonly IConsentData _consentData;

        public LevelPlayHandler(IAdsService adsService, IAdProviderCallbacks callbacks, IServiceLogger logger, IConsentData consentData)
            : base(adsService, callbacks, logger)
        {
            _consentData = consentData;
        }

        public override string ProviderName => "LevelPlay";

        private LevelPlayContainer Container => _adsSettings.GetContainer<LevelPlayContainer>();

        private LevelPlayBannerAd _bannerAd;
        private LevelPlayInterstitialAd _interstitialAd;
        private LevelPlayRewardedAd _rewardedVideoAd;

        private UniTaskCompletionSource<bool> _loadingTask;

        protected override async UniTask<bool> InitProviderAsync()
        {
            _logger.Log("[AdsManager]: LevelPlay is trying to initialize!");

            LevelPlay.ValidateIntegration();

            _loadingTask = new UniTaskCompletionSource<bool>();

            LevelPlay.OnInitSuccess += OnInitSucces;
            LevelPlay.OnInitFailed += OnInitFailed;

            LevelPlay.SetConsent(_consentData.IsConsentGiven);

            if (_adsService.DebugMode)
                LevelPlay.SetMetaData("is_test_suite", "enable");

            LevelPlay.Init(GetAppKey());

            return await _loadingTask.Task;
        }

        private void OnInitFailed(LevelPlayInitError error)
        {
            Debug.LogError($"[AdsManager]: LevelPlay failed to initialized! Error: #{error.ErrorCode} {error.ErrorMessage}");

            _loadingTask?.TrySetResult(false);
        }

        private void OnInitSucces(LevelPlayConfiguration configuration)
        {
            _logger.Log("[AdsManager]: LevelPlay is initialized!");

            OnInitCompleted();

            _loadingTask?.TrySetResult(true);

            LevelPlay.OnInitSuccess -= OnInitSucces;
            LevelPlay.OnInitFailed -= OnInitFailed;
        }

        private void OnInitCompleted()
        {
            var bannerSize = LevelPlayAdSize.BANNER;
            switch (Container.BannerType)
            {
                case LevelPlayContainer.BannerPlacementType.Large:
                    bannerSize = LevelPlayAdSize.LARGE;
                    break;
                case LevelPlayContainer.BannerPlacementType.Rectangle:
                    bannerSize = LevelPlayAdSize.MEDIUM_RECTANGLE;
                    break;
                case LevelPlayContainer.BannerPlacementType.Leaderboard:
                    bannerSize = LevelPlayAdSize.LEADERBOARD;
                    break;
            }

            var bannerPosition = LevelPlayBannerPosition.BottomCenter;
            if (Container.BannerPosition == BannerPosition.Top)
                bannerPosition = LevelPlayBannerPosition.TopCenter;

            var bannerConfig = new LevelPlayBannerAd.Config.Builder().SetPosition(bannerPosition).SetSize(bannerSize).Build();

            _bannerAd = new LevelPlayBannerAd(GetBannerID(), bannerConfig);
            _bannerAd.OnAdLoaded += BannerOnAdLoadedEvent;
            _bannerAd.OnAdDisplayed += BannerOnAdDisplayedEvent;
            _bannerAd.LoadAd();

            _interstitialAd = new LevelPlayInterstitialAd(GetInterstitialID());

            _interstitialAd.OnAdLoaded += InterstitialOnAdLoadedEvent;
            _interstitialAd.OnAdLoadFailed += InterstitialOnAdLoadFailedEvent;
            _interstitialAd.OnAdDisplayed += InterstitialOnAdDisplayedEvent;
            _interstitialAd.OnAdDisplayFailed += InterstitialOnAdDisplayFailedEvent;
            _interstitialAd.OnAdClosed += InterstitialOnAdClosedEvent;

            _rewardedVideoAd = new LevelPlayRewardedAd(GetRewardedVideoID());

            _rewardedVideoAd.OnAdDisplayed += RewardedVideoOnAdOpenedEvent;
            _rewardedVideoAd.OnAdDisplayFailed += RewardedVideoOnAdShowFailedEvent;
            _rewardedVideoAd.OnAdRewarded += RewardedVideoOnAdRewardedEvent;
            _rewardedVideoAd.OnAdClosed += RewardedVideoOnAdClosedEvent;

            if (_adsService.DebugMode)
                OpenTestSuite();
        }

        #region Rewarded Ad
        public override void RequestRewardedVideo()
        {
            if (_rewardedVideoAd != null)
                _rewardedVideoAd.LoadAd();
        }

        public override void ShowRewardedVideo(AdvertisementCallback callback)
        {
            if (_rewardedVideoAd != null)
                _rewardedVideoAd.ShowAd();
        }

        public override bool IsRewardedVideoLoaded()
        {
            return _rewardedVideoAd != null && _rewardedVideoAd.IsAdReady();
        }

        private void RewardedVideoOnAdOpenedEvent(LevelPlayAdInfo adInfo)
        {
            _callbacks.CallEventInMainThread(delegate
            {
                AudioListener.pause = true;

                _logger.Log("[AdsManager]: RewardedVideoOnAdOpenedEvent event received");

                _callbacks.OnProviderAdDisplayed(ProviderName, AdType.RewardedVideo);
            });
        }

        private void RewardedVideoOnAdClosedEvent(LevelPlayAdInfo info)
        {
            _callbacks.CallEventInMainThread(delegate
            {
                AudioListener.pause = false;

                _logger.Log("[AdsManager]: RewardedVideoOnAdClosedEvent event received");

                _callbacks.OnProviderAdClosed(ProviderName, AdType.RewardedVideo);
            });
        }

        private void RewardedVideoOnAdRewardedEvent(LevelPlayAdInfo info, LevelPlayReward reward)
        {
            _callbacks.CallEventInMainThread(delegate
            {
                _callbacks.ExecuteRewardVideoCallback(true);

                _logger.Log("[AdsManager]: RewardedVideoOnAdRewardedEvent event received");

                _adsService.ResetInterstitialDelayTime();
                _adsService.RequestRewardBasedVideo();
            });
        }

        private void RewardedVideoOnAdShowFailedEvent(LevelPlayAdInfo info, LevelPlayAdError error)
        {
            _callbacks.CallEventInMainThread(delegate
            {
                _callbacks.ExecuteRewardVideoCallback(false);

                _logger.Log("[AdsManager]: RewardedVideoOnAdShowFailedEvent event received with message: " + error);

                HandleAdLoadFailure(AdType.RewardedVideo, error.ErrorMessage, ref _rewardedRetryAttempt, () => RequestRewardedVideo());
            });
        }

        public string GetRewardedVideoID()
        {
#if UNITY_EDITOR
            return "unused";
#elif UNITY_ANDROID
            return Container.AndroidRVID;
#elif UNITY_IOS
            return Container.IOSRVID;
#else
            return "unexpected_platform";
#endif
        }
        #endregion

        #region Interstitial
        public override void RequestInterstitial()
        {
            if (_interstitialAd != null)
                _interstitialAd.LoadAd();
        }

        public override void ShowInterstitial(AdvertisementCallback callback)
        {
            if(_interstitialAd != null)
                _interstitialAd.ShowAd();
        }

        public override bool IsInterstitialLoaded()
        {
            return _interstitialAd != null && _interstitialAd.IsAdReady();
        }

        private void InterstitialOnAdClosedEvent(LevelPlayAdInfo info)
        {
            _callbacks.CallEventInMainThread(delegate
            {
                AudioListener.pause = false;

                _logger.Log("[AdsManager]: InterstitialOnAdClosedEvent event received");

                _callbacks.OnProviderAdClosed(ProviderName, AdType.Interstitial);

                _callbacks.ExecuteInterstitialCallback(true);

                _adsService.ResetInterstitialDelayTime();
                _adsService.RequestInterstitial();
            });
        }

        private void InterstitialOnAdDisplayFailedEvent(LevelPlayAdInfo info, LevelPlayAdError error)
        {
            _callbacks.CallEventInMainThread(delegate
            {
                _logger.Log("[AdsManager]: Interstitial ad failed to load an ad with error: " + error.ErrorMessage);

                HandleAdLoadFailure(AdType.Interstitial, error.ErrorMessage, ref _interstitialRetryAttempt, () => RequestInterstitial());
            });
        }

        private void InterstitialOnAdDisplayedEvent(LevelPlayAdInfo info)
        {
            _callbacks.CallEventInMainThread(delegate
            {
                AudioListener.pause = true;

                _logger.Log("[AdsManager]: InterstitialOnAdDisplayedEvent event received");

                _callbacks.OnProviderAdDisplayed(ProviderName, AdType.Interstitial);
            });
        }

        private void InterstitialOnAdLoadFailedEvent(LevelPlayAdError error)
        {
            _callbacks.CallEventInMainThread(delegate
            {
                _logger.Log("[AdsManager]: Interstitial ad failed to load an ad with error: " + error.ErrorMessage);

                HandleAdLoadFailure(AdType.Interstitial, error.ErrorMessage, ref _interstitialRetryAttempt, () => RequestInterstitial());
            });
        }

        private void InterstitialOnAdLoadedEvent(LevelPlayAdInfo info)
        {
            _callbacks.CallEventInMainThread(delegate
            {
                _logger.Log("[AdsManager]: Interstitial ad loaded");

                _interstitialRetryAttempt = RetryAttemptDefaultValue;

                _callbacks.OnProviderAdLoaded(ProviderName, AdType.Interstitial);
            });
        }

        public string GetInterstitialID()
        {
#if UNITY_EDITOR
            return "unused";
#elif UNITY_ANDROID
            return Container.AndroidInterstitialID;
#elif UNITY_IOS
            return Container.IOSInterstitialID;
#else
            return "unexpected_platform";
#endif
        }
        #endregion

        #region Banner
        public override void DestroyBanner()
        {
            if(_bannerAd != null)
            {
                _bannerAd.DestroyAd();
                _bannerAd = null;

                _callbacks.OnProviderAdClosed(ProviderName, AdType.Banner);
            }
        }

        public override void HideBanner()
        {
            if (_bannerAd != null)
            {
                _bannerAd.HideAd();

                _callbacks.OnProviderAdClosed(ProviderName, AdType.Banner);
            }
        }

        public override void ShowBanner()
        {
            if(_bannerAd != null)
            {
                _bannerAd.ShowAd();
            }
        }

        private void BannerOnAdLoadedEvent(LevelPlayAdInfo info)
        {
            _callbacks.CallEventInMainThread(delegate
            {
                _logger.Log("[AdsManager]: BannerOnAdLoadedEvent event received");

                _callbacks.OnProviderAdLoaded(ProviderName, AdType.Banner);
            });
        }

        private void BannerOnAdDisplayedEvent(LevelPlayAdInfo info)
        {
            _callbacks.CallEventInMainThread(delegate
            {
                _logger.Log("[AdsManager]: BannerOnAdDisplayedEvent event received");

                _callbacks.OnProviderAdDisplayed(ProviderName, AdType.Banner);
            });
        }

        public string GetBannerID()
        {
#if UNITY_EDITOR
            return "unused";
#elif UNITY_ANDROID
            return Container.AndroidBannerID;
#elif UNITY_IOS
            return Container.IOSBannerID;
#else
            return "unexpected_platform";
#endif
        }
        #endregion

        public void Unload()
        {
            if (_interstitialAd != null)
            {
                _interstitialAd.OnAdLoaded -= InterstitialOnAdLoadedEvent;
                _interstitialAd.OnAdLoadFailed -= InterstitialOnAdLoadFailedEvent;
                _interstitialAd.OnAdDisplayed -= InterstitialOnAdDisplayedEvent;
                _interstitialAd.OnAdDisplayFailed -= InterstitialOnAdDisplayFailedEvent;
                _interstitialAd.OnAdClosed -= InterstitialOnAdClosedEvent;
                _interstitialAd = null;
            }

            if (_rewardedVideoAd != null)
            {
                _rewardedVideoAd.OnAdDisplayed -= RewardedVideoOnAdOpenedEvent;
                _rewardedVideoAd.OnAdDisplayFailed -= RewardedVideoOnAdShowFailedEvent;
                _rewardedVideoAd.OnAdRewarded -= RewardedVideoOnAdRewardedEvent;
                _rewardedVideoAd.OnAdClosed -= RewardedVideoOnAdClosedEvent;
                _rewardedVideoAd = null;
            }

            if (_bannerAd != null)
            {
                _bannerAd.OnAdLoaded -= BannerOnAdLoadedEvent;
                _bannerAd.OnAdDisplayed -= BannerOnAdDisplayedEvent;
                _bannerAd.DestroyAd();
                _bannerAd = null;
            }
        }

        public void OpenTestSuite()
        {
            LevelPlay.LaunchTestSuite();
        }

        public string GetAppKey()
        {
#if UNITY_EDITOR
            return "unused";
#elif UNITY_ANDROID
            return Container.AndroidAppKey;
#elif UNITY_IOS
            return Container.IOSAppKey;
#else
            return "unexpected_platform";
#endif
        }
    }
#endif
}