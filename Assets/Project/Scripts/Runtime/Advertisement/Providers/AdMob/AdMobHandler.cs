using System;
using Cysharp.Threading.Tasks;
using SBabchuk.Runtime.Services.Contracts;

#if MODULE_ADMOB
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
#endif

namespace SBabchuk.Runtime.Advertisement
{
#if MODULE_ADMOB
    [UnityEngine.Scripting.Preserve]
    public sealed class AdMobHandler : AdProviderHandler
    {
        public AdMobHandler(IAdsService adsService, IAdProviderCallbacks callbacks, IServiceLogger logger)
            : base(adsService, callbacks, logger)
        {
        }

        public override string ProviderName => "AdMob";

        private AdMobContainer Container => _adsSettings.GetContainer<AdMobContainer>();

        private BannerView _bannerView;
        private InterstitialAd _interstitial;
        private RewardedAd _rewardBasedVideo;
        private AppOpenAd _appOpenAd;

        private int _appOpenRetryAttempt = RetryAttemptDefaultValue;

        private DateTime _appOpenAdExpireTime = new DateTime();
        private bool _appOpenAdCanShow = true;

        protected override async UniTask<bool> InitProviderAsync()
        {
            var tcs = new UniTaskCompletionSource<bool>();

            MobileAds.SetiOSAppPauseOnBackground(true);

            var requestConfiguration = new RequestConfiguration()
            {
                TagForChildDirectedTreatment = TagForChildDirectedTreatment.Unspecified,
                TestDeviceIds = _adsSettings.TestDevices
            };

            MobileAds.SetRequestConfiguration(requestConfiguration);

            MobileAds.Initialize(initStatus =>
            {
                if (initStatus != null)
                {
                    tcs.TrySetResult(true);

                    _callbacks.CallEventInMainThread(() =>
                    {
                        if (Container.UseAppOpenAd)
                        {
                            LoadAppOpenAd();
                            AppStateEventNotifier.AppStateChanged += OnAppStateChanged;
                        }
                    });
                }
                else
                {
                    tcs.TrySetResult(false);
                }
            });

            return await tcs.Task;
        }

        public AdRequest GetAdRequest()
        {
            return new AdRequest();
        }

        #region Banner
        public override void DestroyBanner()
        {
            _bannerView?.Destroy();
        }

        public override void HideBanner()
        {
            _bannerView?.Hide();
        }

        private void RequestBanner()
        {
            _bannerView?.Destroy();

            var adSize = GetAdSize();
            var adPosition = GetAdPosition();

            _bannerView = new BannerView(GetBannerID(), adSize, adPosition);

            RegisterBannerEvents(_bannerView);

            _bannerView.LoadAd(GetAdRequest());
        }

        public override void ShowBanner()
        {
            if (_bannerView == null)
            {
                RequestBanner();
            }

            _bannerView?.Show();
        }

        private void RegisterBannerEvents(BannerView banner)
        {
            banner.OnBannerAdLoaded += HandleAdLoaded;
            banner.OnBannerAdLoadFailed += HandleAdFailedToLoad;
            banner.OnAdPaid += HandleAdPaid;
            banner.OnAdClicked += HandleAdClicked;
            banner.OnAdFullScreenContentClosed += HandleAdClosed;
        }

        public void HandleAdLoaded()
        {
            _callbacks.CallEventInMainThread(() =>
            {
                _logger.Log("[AdsManager]: Banner ad loaded");

                _callbacks.OnProviderAdLoaded(ProviderName, AdType.Banner);
            });
        }

        public void HandleAdFailedToLoad(LoadAdError error)
        {
            _callbacks.CallEventInMainThread(() =>
            {
                _logger.LogError($"[AdsManager]: Failed to load banner ad. Error: {error.GetMessage()}");
            });
        }

        private void HandleAdClicked()
        {
            _callbacks.CallEventInMainThread(() =>
            {
                _logger.Log("[AdsManager]: Banner ad clicked");
            });
        }

        private void HandleAdClosed()
        {
            _callbacks.CallEventInMainThread(() =>
            {
                _logger.Log("[AdsManager]: Banner ad closed");

                _callbacks.OnProviderAdClosed(ProviderName, AdType.Banner);
            });
        }

        private void HandleAdPaid(AdValue adValue)
        {
            _callbacks.CallEventInMainThread(() =>
            {
                _logger.Log($"[AdsManager]: Banner ad paid {adValue.Value} {adValue.CurrencyCode}");
            });
        }

        private AdSize GetAdSize()
        {
            return Container.BannerType switch
            {
                AdMobContainer.BannerPlacementType.MediumRectangle => AdSize.MediumRectangle,
                AdMobContainer.BannerPlacementType.IABBanner => AdSize.IABBanner,
                AdMobContainer.BannerPlacementType.Leaderboard => AdSize.Leaderboard,
                _ => AdSize.Banner,
            };
        }

        private AdPosition GetAdPosition()
        {
            return Container.BannerPosition switch
            {
                BannerPosition.Top => AdPosition.Top,
                _ => AdPosition.Bottom,
            };
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

        #region Interstitial
        public override void RequestInterstitial()
        {
            _interstitial?.Destroy();

            InterstitialAd.Load(GetInterstitialID(), GetAdRequest(), (InterstitialAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    HandleAdLoadFailure(AdType.Interstitial, error.GetMessage(), ref _interstitialRetryAttempt, RequestInterstitial);

                    return;
                }

                _interstitial = ad;
                _interstitialRetryAttempt = RetryAttemptDefaultValue;

                _callbacks.OnProviderAdLoaded(ProviderName, AdType.Interstitial);

                RegisterInterstitialEvents(_interstitial);
            });
        }

        public override void ShowInterstitial(AdvertisementCallback callback)
        {
            _interstitial?.Show();
        }

        private void RegisterInterstitialEvents(InterstitialAd ad)
        {
            ad.OnAdFullScreenContentOpened += HandleInterstitialOpened;
            ad.OnAdFullScreenContentClosed += HandleInterstitialClosed;
            ad.OnAdClicked += HandleInterstitialClicked;
        }

        private void HandleInterstitialOpened()
        {
            _callbacks.CallEventInMainThread(() =>
            {
                _appOpenAdCanShow = false;

                _logger.Log("[AdsManager]: Interstitial ad opened");

                _callbacks.OnProviderAdDisplayed(ProviderName, AdType.Interstitial);
            });
        }

        private void HandleInterstitialClosed()
        {
            _callbacks.CallEventInMainThread(() =>
            {
                _appOpenAdCanShow = false;

                _logger.Log("[AdsManager]: Interstitial ad closed");

                _callbacks.OnProviderAdClosed(ProviderName, AdType.Interstitial);
                _callbacks.ExecuteInterstitialCallback(true);

                _adsService.ResetInterstitialDelayTime();

                RequestInterstitial();
            });
        }

        private void HandleInterstitialClicked()
        {
            _callbacks.CallEventInMainThread(() =>
            {
                _logger.Log("[AdsManager]: Interstitial ad clicked");
            });
        }

        public override bool IsInterstitialLoaded()
        {
            return _interstitial != null && _interstitial.CanShowAd();
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

        #region Rewarded Video
        public override void RequestRewardedVideo()
        {
            RewardedAd.Load(GetRewardedVideoID(), GetAdRequest(), (RewardedAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    HandleAdLoadFailure(AdType.RewardedVideo, error.GetMessage(), ref _rewardedRetryAttempt, RequestRewardedVideo);

                    return;
                }

                _rewardBasedVideo = ad;
                _rewardedRetryAttempt = RetryAttemptDefaultValue;

                _callbacks.OnProviderAdLoaded(ProviderName, AdType.RewardedVideo);

                RegisterRewardedVideoEvents(_rewardBasedVideo);
            });
        }

        public override void ShowRewardedVideo(AdvertisementCallback callback)
        {
            _rewardBasedVideo?.Show(reward =>
            {
                _callbacks.CallEventInMainThread(() =>
                {
                    _callbacks.OnProviderAdDisplayed(ProviderName, AdType.RewardedVideo);
                    _callbacks.ExecuteRewardVideoCallback(true);

                    _logger.Log("[AdsManager]: Rewarded video completed");

                    _adsService.ResetInterstitialDelayTime();
                    RequestRewardedVideo();
                });
            });
        }

        private void RegisterRewardedVideoEvents(RewardedAd ad)
        {
            ad.OnAdFullScreenContentFailed += HandleRewardBasedVideoFailedToShow;
            ad.OnAdFullScreenContentOpened += HandleRewardBasedVideoOpened;
            ad.OnAdFullScreenContentClosed += HandleRewardBasedVideoClosed;
            ad.OnAdClicked += HandleRewardBasedVideoClicked;
        }

        private void HandleRewardBasedVideoFailedToShow(AdError error)
        {
            _callbacks.CallEventInMainThread(() =>
            {
                _callbacks.ExecuteRewardVideoCallback(false);

                HandleAdLoadFailure(AdType.RewardedVideo, error.GetMessage(), ref _rewardedRetryAttempt, RequestRewardedVideo);
            });
        }

        private void HandleRewardBasedVideoOpened()
        {
            _callbacks.CallEventInMainThread(() =>
            {
                _appOpenAdCanShow = false;

                _logger.Log("[AdsManager]: Rewarded video opened");
            });
        }

        private void HandleRewardBasedVideoClosed()
        {
            _callbacks.CallEventInMainThread(() =>
            {
                _appOpenAdCanShow = false;

                _logger.Log("[AdsManager]: Rewarded video closed");

                _callbacks.OnProviderAdClosed(ProviderName, AdType.RewardedVideo);
            });
        }

        private void HandleRewardBasedVideoClicked()
        {
            _callbacks.CallEventInMainThread(() =>
            {
                _logger.Log("[AdsManager]: Rewarded video clicked");
            });
        }

        public override bool IsRewardedVideoLoaded()
        {
            return _rewardBasedVideo != null && _rewardBasedVideo.CanShowAd();
        }

        public string GetRewardedVideoID()
        {
#if UNITY_EDITOR
            return "unused";
#elif UNITY_ANDROID
            return Container.AndroidRewardedVideoID;
#elif UNITY_IOS
            return Container.IOSRewardedVideoID;
#else
            return "unexpected_platform";
#endif
        }
        #endregion

        #region App Open Ad
        private bool IsAppOpenAdAvailable()
        {
            return _appOpenAd != null && _appOpenAd.CanShowAd() && DateTime.Now < _appOpenAdExpireTime;
        }

        private void OnAppStateChanged(AppState state)
        {
            _logger.Log($"[AdsManager]: App State changed to : {state}");

            if(state == AppState.Foreground)
            {
                if (_appOpenAdCanShow && IsAppOpenAdAvailable())
                {
                    _appOpenAd.Show();

                    _adsService.DisableBanner();
                }

                _appOpenAdCanShow = true;
            }
        }

        private void LoadAppOpenAd()
        {
            _appOpenAd?.Destroy();
            _appOpenAd = null;

            AppOpenAd.Load(GetAppOpenID(), GetAdRequest(), (AppOpenAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    HandleAdLoadFailure(AdType.AppOpen, error != null ? error.GetMessage() : "Ad is null", ref _appOpenRetryAttempt, LoadAppOpenAd);
                    return;
                }

                _appOpenAd = ad;
                _appOpenRetryAttempt = RetryAttemptDefaultValue;
                _appOpenAdExpireTime = DateTime.Now + TimeSpan.FromHours(Container.AppOpenAdExpirationHoursTime);

                RegisterAppOpenAdEvents(ad);
            });
        }

        private void RegisterAppOpenAdEvents(AppOpenAd ad)
        {
            ad.OnAdPaid += HandleAppOpenPaid;
            ad.OnAdImpressionRecorded += HandleAppOpenImpressionRecorded;
            ad.OnAdClicked += HandleAppOpenAdClicked;
            ad.OnAdFullScreenContentOpened += HandleAppOpenAdContentOpened;
            ad.OnAdFullScreenContentClosed += HandleAppOpenAdContentClosed;
            ad.OnAdFullScreenContentFailed += HandleAppOpenAdConentFailed;
        }

        private void HandleAppOpenAdConentFailed(AdError error)
        {
            _logger.LogError($"[AdsManager]: App open ad failed to open with error: {error}");

            LoadAppOpenAd();

            _adsService.EnableBanner();
        }

        private void HandleAppOpenAdContentClosed()
        {
            _logger.Log("[AdsManager]: App open ad closed.");

            LoadAppOpenAd();

            _adsService.EnableBanner();
        }

        private void HandleAppOpenAdContentOpened()
        {
            _logger.Log("[AdsManager]: App open ad opened.");
        }

        private void HandleAppOpenAdClicked()
        {
            _logger.Log("[AdsManager]: App open ad clicked.");
        }

        private void HandleAppOpenImpressionRecorded()
        {
            _logger.Log("[AdsManager]: App open ad recorded an impression.");
        }

        private void HandleAppOpenPaid(AdValue value)
        {
            _logger.Log($"[AdsManager]: App open ad paid {value.Value} {value.CurrencyCode}.");
        }

        public string GetAppOpenID()
        {
#if UNITY_EDITOR
            return "unused";
#elif UNITY_ANDROID
            return Container.AndroidAppOpenAdID;
#elif UNITY_IOS
            return Container.IOSAppOpenAdID;
#else
            return "unexpected_platform";
#endif
        }
        #endregion
    }
#endif
}
