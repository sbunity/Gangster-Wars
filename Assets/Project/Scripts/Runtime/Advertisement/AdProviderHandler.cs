using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using UnityEngine.Scripting;

namespace SBabchuk.Runtime.Advertisement
{
    [Preserve]
    public abstract class AdProviderHandler
    {
        protected const int RetryAttemptDefaultValue = 1;
        protected const int MaxRetryAttempts = 5;
        protected const float RetryDelayBase = 2f;

        protected readonly IAdsService _adsService;
        protected readonly IAdProviderCallbacks _callbacks;
        protected readonly IServiceLogger _logger;

        protected int _interstitialRetryAttempt = RetryAttemptDefaultValue;
        protected int _rewardedRetryAttempt = RetryAttemptDefaultValue;

        protected AdsSettings _adsSettings;

        protected AdProviderHandler(IAdsService adsService, IAdProviderCallbacks callbacks, IServiceLogger logger)
        {
            _adsService = adsService;
            _callbacks = callbacks;
            _logger = logger;
        }

        public abstract string ProviderName { get; }

        public bool IsInitialized { get; private set; }

        public void LinkSettings(AdsSettings adsSettings)
        {
            _adsSettings = adsSettings;
        }

        public async UniTask<bool> InitAsync()
        {
            if (IsInitialized)
                return true;

            var initResult = await InitProviderAsync();
            await UniTask.SwitchToMainThread();

            if (initResult)
            {
                IsInitialized = true;

                _callbacks.OnProviderInitialized(ProviderName);

                _logger.Log(string.Format("[AdsManager]: {0} is initialized!", ProviderName));

                return true;
            }

            return false;
        }

        protected abstract UniTask<bool> InitProviderAsync();

        public abstract void ShowBanner();
        public abstract void HideBanner();
        public abstract void DestroyBanner();

        public abstract void RequestInterstitial();
        public abstract void ShowInterstitial(AdvertisementCallback callback);
        public abstract bool IsInterstitialLoaded();

        public abstract void RequestRewardedVideo();
        public abstract void ShowRewardedVideo(AdvertisementCallback callback);
        public abstract bool IsRewardedVideoLoaded();

        public delegate void AdvertisementCallback(bool result);

        protected void HandleAdLoadFailure(AdType adType, string errorMessage, ref int retryAttempt, Action retryAction)
        {
            _logger.LogError($"[AdsManager]: {adType} failed to load with error: {errorMessage}");

            retryAttempt++;
            if (retryAttempt <= MaxRetryAttempts)
            {
                var retryDelay = Mathf.Pow(RetryDelayBase, retryAttempt);
                DOVirtual.DelayedCall(retryDelay, () => retryAction(), true);
            }
            else
            {
                Debug.LogError($"[AdsManager]: {adType} failed after {MaxRetryAttempts} retries.");
            }
        }
    }
}
