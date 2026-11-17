using Cysharp.Threading.Tasks;
using DG.Tweening;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;

namespace SBabchuk.Runtime.Advertisement
{
#if MODULE_APPLOVIN
    [UnityEngine.Scripting.Preserve]
    public sealed class ApplovinHandler : AdProviderHandler
    {
        public ApplovinHandler(IAdsService adsService, IAdProviderCallbacks callbacks, IServiceLogger logger)
            : base(adsService, callbacks, logger)
        {
        }

        public override string ProviderName => "Applovin";

        private ApplovinContainer Container => _adsSettings.GetContainer<ApplovinContainer>();

        private bool _isBannerLoaded;

        protected override async UniTask<bool> InitProviderAsync()
        {
            var tcs = new UniTaskCompletionSource<bool>();

            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += RewardedVideoOnAdLoadedEvent;
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += RewardedVideoOnAdLoadFailedEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += RewardedVideoOnAdOpenedEvent;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += RewardedVideoOnAdClosedEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += RewardedVideoOnAdShowFailedEvent;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += RewardedVideoOnAdRewardedEvent;

            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += InterstitialOnAdReadyEvent;
            MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += InterstitialOnAdLoadFailed;
            MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += InterstitialOnAdOpenedEvent;
            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += InterstitialOnAdClosedEvent;
            MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += InterstitialOnAdShowFailedEvent;

            MaxSdkCallbacks.Banner.OnAdLoadedEvent += BannerOnAdLoadedEvent;

            MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
            {
                tcs.TrySetResult(true);
            };

            MaxSdk.InitializeSdk();

            return await tcs.Task;
        }

        #region RewardedAd callback handlers
        private void RewardedVideoOnAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            _callbacks.CallEventInMainThread(delegate
            {
                _logger.Log("[AdsManager]: Rewarded ad loaded");

                _rewardedRetryAttempt = RetryAttemptDefaultValue;

                _callbacks.OnProviderAdLoaded(ProviderName, AdType.RewardedVideo);
            });
        }

        private void RewardedVideoOnAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            _callbacks.CallEventInMainThread(delegate
            {
                _logger.Log("[AdsManager]: Rewarded ad failed to load with error: " + errorInfo);

                _rewardedRetryAttempt++;
                var retryDelay = Mathf.Pow(RetryDelayBase, _rewardedRetryAttempt);

                DOVirtual.DelayedCall(retryDelay, () => _adsService.RequestRewardBasedVideo(), true);
            });
        }

        private void RewardedVideoOnAdOpenedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            _callbacks.CallEventInMainThread(delegate
            {
                _logger.Log("[AdsManager]: RewardedVideoOnAdOpenedEvent event received");

                _callbacks.OnProviderAdDisplayed(ProviderName, AdType.RewardedVideo);
            });
        }

        private void RewardedVideoOnAdClosedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            _callbacks.CallEventInMainThread(delegate
            {
                _callbacks.ExecuteRewardVideoCallback(false);

                _logger.Log("[AdsManager]: RewardedVideoOnAdClosedEvent event received");

                _callbacks.OnProviderAdClosed(ProviderName, AdType.RewardedVideo);

                _adsService.RequestRewardBasedVideo();
            });
        }

        private void RewardedVideoOnAdRewardedEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
        {
            _callbacks.CallEventInMainThread(delegate
            {
                _callbacks.ExecuteRewardVideoCallback(true);

                _logger.Log("[AdsManager]: RewardedVideoOnAdRewardedEvent event received");

                _adsService.ResetInterstitialDelayTime();
            });
        }

        private void RewardedVideoOnAdShowFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            _callbacks.CallEventInMainThread(delegate
            {
                _callbacks.ExecuteRewardVideoCallback(false);

                _logger.Log("[AdsManager]: RewardedVideoOnAdShowFailedEvent event received with message: " + errorInfo);

                _rewardedRetryAttempt++;
                var retryDelay = Mathf.Pow(RetryDelayBase, _rewardedRetryAttempt);

                DOVirtual.DelayedCall(retryDelay, () => _adsService.RequestRewardBasedVideo(), true);
            });
        }
        #endregion

        #region Interstitial callback handlers
        private void InterstitialOnAdReadyEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            _callbacks.CallEventInMainThread(delegate
            {
                _logger.Log("[AdsManager]: Interstitial ad loaded");

                _interstitialRetryAttempt = RetryAttemptDefaultValue;

                _callbacks.OnProviderAdLoaded(ProviderName, AdType.Interstitial);
            });
        }

        private void InterstitialOnAdLoadFailed(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            _callbacks.CallEventInMainThread(delegate
            {
                _logger.Log("[AdsManager]: Interstitial ad failed to load an ad with error: " + errorInfo);

                _interstitialRetryAttempt++;
                var retryDelay = Mathf.Pow(RetryDelayBase, _interstitialRetryAttempt);

                DOVirtual.DelayedCall(retryDelay, () => _adsService.RequestInterstitial(), true);
            });
        }

        private void InterstitialOnAdOpenedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            _callbacks.CallEventInMainThread(delegate
            {
                _logger.Log("[AdsManager]: InterstitialOnAdOpenedEvent event received");

                _callbacks.OnProviderAdDisplayed(ProviderName, AdType.Interstitial);
            });
        }

        private void InterstitialOnAdShowFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            _callbacks.CallEventInMainThread(delegate
            {
                _logger.Log("[AdsManager]: Interstitial ad failed to show with error: " + errorInfo);

                _interstitialRetryAttempt++;
                var retryDelay = Mathf.Pow(RetryDelayBase, _interstitialRetryAttempt);

                DOVirtual.DelayedCall(retryDelay, () => _adsService.RequestInterstitial(), true);
            });
        }

        private void InterstitialOnAdClosedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            _callbacks.CallEventInMainThread(delegate
            {
                _logger.Log("[AdsManager]: InterstitialOnAdClosedEvent event received");

                _callbacks.OnProviderAdClosed(ProviderName, AdType.Interstitial);

                _callbacks.ExecuteInterstitialCallback(true);

                _adsService.ResetInterstitialDelayTime();
                _adsService.RequestInterstitial();
            });
        }
        #endregion

        #region Banner callback handlers
        private void BannerOnAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            _callbacks.CallEventInMainThread(delegate
            {
                _logger.Log("[AdsManager]: BannerOnAdLoadedEvent event received");

                _callbacks.OnProviderAdLoaded(ProviderName, AdType.Banner);

                if(Container.UseAdaptiveBanner)
                {
                    var adapsiveHeight = MaxSdkUtils.GetAdaptiveBannerHeight();
                    var screenPixelHeight = adapsiveHeight * MaxSdkUtils.GetScreenDensity();

                    if (screenPixelHeight > 0)
                    {
                        _adsService.SetBannerHeight(screenPixelHeight);
                    }
                    else
                    {
                        _adsService.ResetBannerHeight();
                    }
                }
                else
                {
                    var layout = MaxSdk.GetBannerLayout(adUnitId);

                    var screenPixelHeight = layout.height * MaxSdkUtils.GetScreenDensity();
                    if (screenPixelHeight > 0)
                    {
                        _adsService.SetBannerHeight(screenPixelHeight);
                    }
                    else
                    {
                        _adsService.ResetBannerHeight();
                    }
                }
            });
        }
        #endregion

        public override void DestroyBanner()
        {
            MaxSdk.DestroyBanner(GetBannerID());

            _isBannerLoaded = false;

            _callbacks.OnProviderAdClosed(ProviderName, AdType.Banner);
        }

        public override void HideBanner()
        {
            if (_isBannerLoaded)
                MaxSdk.HideBanner(GetBannerID());

            _callbacks.OnProviderAdClosed(ProviderName, AdType.Banner);
        }

        public override void ShowBanner()
        {
            if (!_isBannerLoaded)
            {
                MaxSdkBase.AdViewConfiguration bannerConfiguration = new MaxSdkBase.AdViewConfiguration((MaxSdkBase.AdViewPosition)Container.BannerPosition);
                bannerConfiguration.IsAdaptive = Container.UseAdaptiveBanner;

                MaxSdk.CreateBanner(GetBannerID(), bannerConfiguration);

                MaxSdk.SetBannerBackgroundColor(GetBannerID(), Container.BannerBackgroundColor);

                _isBannerLoaded = true;
            }
            else
            {
                MaxSdk.ShowBanner(GetBannerID());

            }

            _callbacks.OnProviderAdDisplayed(ProviderName, AdType.Banner);
        }

        public override void RequestInterstitial()
        {
            MaxSdk.LoadInterstitial(GetInterstitialID());
        }

        public override void ShowInterstitial(AdvertisementCallback callback)
        {
            MaxSdk.ShowInterstitial(GetInterstitialID());
        }

        public override void RequestRewardedVideo()
        {
            MaxSdk.LoadRewardedAd(GetRewardedVideoID());
        }

        public override void ShowRewardedVideo(AdvertisementCallback callback)
        {
            MaxSdk.ShowRewardedAd(GetRewardedVideoID());
        }

        public override bool IsInterstitialLoaded()
        {
            return MaxSdk.IsInterstitialReady(GetInterstitialID());
        }

        public override bool IsRewardedVideoLoaded()
        {
            return MaxSdk.IsRewardedAdReady(GetRewardedVideoID());
        }

        public string GetBannerID()
        {
#if UNITY_ANDROID
            return Container.AndroidBannerID;
#elif UNITY_IOS
            return Container.IOSBannerID;
#else
            return string.Empty;
#endif
        }

        public string GetInterstitialID()
        {
#if UNITY_ANDROID
            return Container.AndroidInterstitialID;
#elif UNITY_IOS
            return Container.IOSInterstitialID;
#else
            return string.Empty;
#endif
        }

        public string GetRewardedVideoID()
        {
#if UNITY_ANDROID
            return Container.AndroidRewardedVideoID;
#elif UNITY_IOS
            return Container.IOSRewardedVideoID;
#else
            return string.Empty;
#endif
        }
    }
#endif
}