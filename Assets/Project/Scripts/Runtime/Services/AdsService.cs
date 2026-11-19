using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using SBabchuk.Runtime.Advertisement;
using SBabchuk.Runtime.Progress;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using Zenject;

namespace SBabchuk.Runtime.Services
{
    public sealed class AdsService : IAdsService, IAdProviderCallbacks, IDisposable
    {
        private const int DefaultBannerHeight = 110;
        private const int InitAttemptsAmount = 30;
        private const double ForcedAdDisabledForever = -1;
        private const float FirstAdsLoadDelay = 1.0f;
        private const float FirstAdsLoadRetryStep = 1.0f;

        private readonly IPlayerProgressRepository _repository;
        private readonly ISystemMessageService _systemMessage;
        private readonly IMainThreadDispatcher _dispatcher;
        private readonly IServiceLogger _logger;
        private readonly IInstantiator _instantiator;
        private readonly CancellationTokenSource _cancellation = new();

        private AdProviderHandler _activeHandler;
        private bool _isInitialized;

        private double _lastInterstitialTime;

        private AdProviderHandler.AdvertisementCallback _rewardedVideoCallback;
        private AdProviderHandler.AdvertisementCallback _interstitalCallback;

        private bool _isFirstAdLoaded;
        private bool _waitingForRewardVideoCallback;
        private bool _isLoadingFirstAds;

        private Tween _delayTween;

        private float _intFirstStartDelay;
        private float _intStartDelay;
        private float _intShowingDelay;

        private AdSave _save;

        public AdsService(
            IPlayerProgressRepository repository,
            ISystemMessageService systemMessage,
            IMainThreadDispatcher dispatcher,
            IServiceLogger logger,
            IInstantiator instantiator)
        {
            _repository = repository;
            _systemMessage = systemMessage;
            _dispatcher = dispatcher;
            _logger = logger;
            _instantiator = instantiator;
        }

        public AdsSettings Settings { get; private set; }
        public bool DebugMode { get; private set; }
        public bool IsBannerActive { get; private set; } = true;
        public float BannerHeight { get; private set; } = DefaultBannerHeight;

        public event Action ForcedAdDisabled;
        public event Action<string> AdProviderInitialized;
        public event Action<string, AdType> AdLoaded;
        public event Action<string, AdType> AdDisplayed;
        public event Action<string, AdType> AdClosed;
        public event Func<bool> InterstitialConditions;

        public void Init(AdsSettings adsSettings)
        {
            if (_isInitialized)
            {
                _logger.LogWarning("[AdsManager]: Module already exists!");
                return;
            }

            _isInitialized = true;

            _isFirstAdLoaded = false;

            DebugMode = adsSettings.DebugMode;

            Settings = adsSettings;

            _intFirstStartDelay = Settings.InterstitialFirstStartDelay;
            _intStartDelay = Settings.InterstitialStartDelay;
            _intShowingDelay = Settings.InterstitialShowingDelay;

            _save = _repository.Progress.Ads;

            if (!_save.IsFirstLaunchPassed)
            {
                _lastInterstitialTime = Time.time + _intFirstStartDelay;
                _save.IsFirstLaunchPassed = true;
                _repository.Save();
            }
            else
            {
                _lastInterstitialTime = Time.time + _intStartDelay;
            }

            if (_save.ForcedAdDisabledUntil != 0 && _save.ForcedAdDisabledUntil != ForcedAdDisabledForever && IsForcedAdEnabled())
                _save.ForcedAdDisabledUntil = 0;

            _activeHandler = Settings.GetContainer(Settings.ActiveProvider)?.CreateHandler(_instantiator);

            if (_activeHandler != null)
                _activeHandler.LinkSettings(Settings);
            else
                Debug.LogError($"[AdsManager]: Provider '{Settings.ActiveProvider}' not found or SDK not installed!");

            InitializeModulesAsync(Settings.LoadAdsOnStart).Forget();
        }

        public void Dispose()
        {
            _delayTween?.Kill();
            _cancellation.Cancel();
            _cancellation.Dispose();
        }

        private async UniTaskVoid InitializeModulesAsync(bool loadAds)
        {
            if (_activeHandler == null)
                return;

            _logger.Log($"[AdsManager]: {_activeHandler.ProviderName} is trying to initialize!");

            var isInitialized = await _activeHandler.InitAsync();

            if (isInitialized)
                _logger.Log($"[AdsManager]: {_activeHandler.ProviderName} initialized successfully.");
            else
                Debug.LogError($"[AdsManager]: {_activeHandler.ProviderName} failed to initialize.");

            if (loadAds)
                TryToLoadFirstAds();
        }

        public void TryToLoadFirstAds()
        {
            if (_isLoadingFirstAds)
                return;

            _isLoadingFirstAds = true;
            _logger.Log("[AdsManager]: Loading first ads..");
            TryToLoadAdsAsync(_cancellation.Token).Forget();
        }

        private async UniTaskVoid TryToLoadAdsAsync(CancellationToken cancellationToken)
        {
            var initAttempts = 0;

            await UniTask.Delay(TimeSpan.FromSeconds(FirstAdsLoadDelay), cancellationToken: cancellationToken);

            while (!_isFirstAdLoaded && initAttempts < InitAttemptsAmount)
            {
                if (LoadFirstAds())
                    break;

                await UniTask.Delay(TimeSpan.FromSeconds(FirstAdsLoadRetryStep * (initAttempts + 1)), cancellationToken: cancellationToken);

                initAttempts++;
            }

            _logger.Log("[AdsManager]: First ads have loaded!");
        }

        private bool LoadFirstAds()
        {
            if (!_isInitialized)
                return false;

            if (_isFirstAdLoaded)
                return true;

            var isProviderInitialized = _activeHandler != null && _activeHandler.IsInitialized;

            _logger.Log($"[AdsManager]: LoadFirstAds — provider={Settings?.ActiveProvider}, initialized={isProviderInitialized}");

            if (!isProviderInitialized)
                return false;

            if (Settings.RewardedVideoEnabled)
                RequestRewardBasedVideo();

            var isForcedAdEnabled = IsForcedAdEnabled();
            _logger.Log($"[AdsManager]: LoadFirstAds — IsForcedAdEnabled={isForcedAdEnabled}");

            if (Settings.InterstitialEnabled && isForcedAdEnabled)
                RequestInterstitial();

            if (Settings.BannerEnabled && isForcedAdEnabled)
                ShowBanner();

            _isFirstAdLoaded = true;

            return true;
        }

        public void CallEventInMainThread(Action callback)
        {
            _dispatcher.Dispatch(callback);
        }

        public void ShowErrorMessage()
        {
            _systemMessage.ShowMessage("Network error. Please try again later");
        }

        public bool IsProviderActive()
        {
            return _isInitialized && _activeHandler != null;
        }

        public bool IsProviderInitialized()
        {
            return _isInitialized && _activeHandler != null && _activeHandler.IsInitialized;
        }

        public AdProviderHandler GetActiveProvider()
        {
            return _activeHandler;
        }

        public bool IsInterstitialLoaded()
        {
            if (!_isInitialized)
            {
                _logger.LogWarning("[AdsManager]: Mobile monetization is disabled!");
                return false;
            }

            if (!IsForcedAdEnabled() || _activeHandler == null)
                return false;

            return _activeHandler.IsInterstitialLoaded();
        }

        public void RequestInterstitial()
        {
            if (!_isInitialized)
            {
                _logger.LogWarning("[AdsManager]: Mobile monetization is disabled!");
                return;
            }

            if (!IsForcedAdEnabled() || _activeHandler == null || !_activeHandler.IsInitialized || _activeHandler.IsInterstitialLoaded())
                return;

            _activeHandler.RequestInterstitial();
        }

        public void ShowInterstitial(AdProviderHandler.AdvertisementCallback callback, bool ignoreConditions = false)
        {
            _interstitalCallback = callback;

            if (!_isInitialized)
            {
                _logger.LogWarning("[AdsManager]: Mobile monetization is disabled!");
                ExecuteInterstitialCallback(false);
                return;
            }

            if (!IsForcedAdEnabled() || _activeHandler == null || (!ignoreConditions && (!CheckInterstitialTime() || !CheckExtraInterstitialCondition())) || !_activeHandler.IsInitialized || !_activeHandler.IsInterstitialLoaded())
            {
                ExecuteInterstitialCallback(false);
                return;
            }

            _delayTween?.Kill();

            if (Settings.LoadingAdDuration > 0)
            {
                _systemMessage.ShowLoadingPanel();
                _systemMessage.ChangeLoadingMessage(Settings.LoadingMessage);

                _delayTween = DOVirtual.DelayedCall(Settings.LoadingAdDuration, () =>
                {
                    _activeHandler.ShowInterstitial(callback);
                    _systemMessage.HideLoadingPanel();
                }, true);
            }
            else
            {
                _activeHandler.ShowInterstitial(callback);
            }
        }

        public void ExecuteInterstitialCallback(bool result)
        {
            if (_interstitalCallback != null)
                CallEventInMainThread(() => _interstitalCallback.Invoke(result));
        }

        public void SetInterstitialDelayTime(float time)
        {
            _lastInterstitialTime = Time.time + time;
        }

        public void ResetInterstitialDelayTime()
        {
            _lastInterstitialTime = Time.time + _intShowingDelay;
        }

        private bool CheckInterstitialTime()
        {
            _logger.Log("[AdsManager]: Interstitial Time: " + _lastInterstitialTime + "; Time: " + Time.time);

            return _lastInterstitialTime < Time.time;
        }

        public bool CheckExtraInterstitialCondition()
        {
            if (InterstitialConditions == null)
                return true;

            var state = true;

            foreach (var condition in InterstitialConditions.GetInvocationList())
            {
                if (!((Func<bool>)condition).Invoke())
                {
                    state = false;
                    break;
                }
            }

            _logger.Log("[AdsManager]: Extra condition interstitial state: " + state);

            return state;
        }

        public bool IsRewardBasedVideoLoaded()
        {
            if (!_isInitialized)
            {
                _logger.LogWarning("[AdsManager]: Mobile monetization is disabled!");
                return false;
            }

            if (_activeHandler == null || !_activeHandler.IsInitialized)
                return false;

            return _activeHandler.IsRewardedVideoLoaded();
        }

        public void RequestRewardBasedVideo()
        {
            if (!_isInitialized)
            {
                _logger.LogWarning("[AdsManager]: Mobile monetization is disabled!");
                return;
            }

            if (!Settings.RewardedVideoEnabled || _activeHandler == null || !_activeHandler.IsInitialized || _activeHandler.IsRewardedVideoLoaded())
                return;

            _activeHandler.RequestRewardedVideo();
        }

        public void ShowRewardBasedVideo(AdProviderHandler.AdvertisementCallback callback, bool showErrorMessage = true)
        {
            _rewardedVideoCallback = callback;
            _waitingForRewardVideoCallback = true;

            if (!_isInitialized)
            {
                _logger.LogWarning("[AdsManager]: Mobile monetization is disabled!");
                ExecuteRewardVideoCallback(false);
                return;
            }

            if (!Settings.RewardedVideoEnabled || _activeHandler == null || !_activeHandler.IsInitialized || !_activeHandler.IsRewardedVideoLoaded())
            {
                ExecuteRewardVideoCallback(false);

                if (showErrorMessage)
                    ShowErrorMessage();

                return;
            }

            _delayTween?.Kill();

            if (Settings.LoadingAdDuration > 0)
            {
                _systemMessage.ShowLoadingPanel();
                _systemMessage.ChangeLoadingMessage(Settings.LoadingMessage);

                _delayTween = DOVirtual.DelayedCall(Settings.LoadingAdDuration, () =>
                {
                    _activeHandler.ShowRewardedVideo(callback);
                    _systemMessage.HideLoadingPanel();
                }, true);
            }
            else
            {
                _activeHandler.ShowRewardedVideo(callback);
            }
        }

        public void ExecuteRewardVideoCallback(bool result)
        {
            if (_rewardedVideoCallback == null || !_waitingForRewardVideoCallback)
                return;

            CallEventInMainThread(() => _rewardedVideoCallback.Invoke(result));

            _waitingForRewardVideoCallback = false;

            _logger.Log("[AdsManager]: Reward received: " + result);
        }

        public void ShowBanner()
        {
            if (!_isInitialized)
            {
                _logger.LogWarning("[AdsManager]: Mobile monetization is disabled!");
                return;
            }

            if (!Settings.BannerEnabled)
            {
                _logger.Log("[AdsManager]: ShowBanner — skipped: bannerEnabled=false");
                return;
            }

            if (!IsBannerActive)
            {
                _logger.Log("[AdsManager]: ShowBanner — skipped: isBannerActive=false");
                return;
            }

            var forcedEnabled = IsForcedAdEnabled();
            var initialized = _activeHandler != null && _activeHandler.IsInitialized;
            _logger.Log($"[AdsManager]: ShowBanner — provider={Settings?.ActiveProvider}, forcedEnabled={forcedEnabled}, initialized={initialized}");

            if (!forcedEnabled || !initialized)
                return;

            _activeHandler.ShowBanner();
        }

        public void DestroyBanner()
        {
            if (!_isInitialized)
            {
                _logger.LogWarning("[AdsManager]: Mobile monetization is disabled!");
                return;
            }

            if (_activeHandler == null || !_activeHandler.IsInitialized)
                return;

            _activeHandler.DestroyBanner();
        }

        public void HideBanner()
        {
            if (!_isInitialized)
            {
                _logger.LogWarning("[AdsManager]: Mobile monetization is disabled!");
                return;
            }

            if (_activeHandler == null || !_activeHandler.IsInitialized)
                return;

            _activeHandler.HideBanner();
        }

        public void EnableBanner()
        {
            if (!_isInitialized)
            {
                _logger.LogWarning("[AdsManager]: Mobile monetization is disabled!");
                return;
            }

            IsBannerActive = true;

            ShowBanner();
        }

        public void DisableBanner()
        {
            if (!_isInitialized)
            {
                _logger.LogWarning("[AdsManager]: Mobile monetization is disabled!");
                return;
            }

            IsBannerActive = false;

            HideBanner();
        }

        public void ResetBannerHeight()
        {
            BannerHeight = DefaultBannerHeight;
        }

        public void SetBannerHeight(float height)
        {
            BannerHeight = height;
        }

        public float GetBannerHeight()
        {
            if (Settings != null && Settings.BannerEnabled && IsForcedAdEnabled() && IsBannerActive)
                return BannerHeight;

            return 0.0f;
        }

        public bool IsForcedAdEnabled()
        {
            if (_save == null)
                return true;

            if (_save.ForcedAdDisabledUntil == ForcedAdDisabledForever)
                return false;

            return GetCurrentUnixTimestamp() >= _save.ForcedAdDisabledUntil;
        }

        public void DisableForcedAdForever()
        {
            DisableForcedAdInternal(ForcedAdDisabledForever);
        }

        public void DisableForcedAd(int durationSeconds)
        {
            if (durationSeconds < 0)
            {
                _logger.LogWarning("[AdsManager]: Invalid duration for ad disable: " + durationSeconds);
                return;
            }

            var targetTime = GetCurrentUnixTimestamp() + durationSeconds;
            if (targetTime <= _save.ForcedAdDisabledUntil)
                return;

            DisableForcedAdInternal(targetTime);
        }

        private void DisableForcedAdInternal(double targetTime)
        {
            _logger.Log("[AdsManager]: Banners and interstitials are disabled!");

            _save.ForcedAdDisabledUntil = targetTime;

            _repository.Save();

            ForcedAdDisabled?.Invoke();

            DestroyBanner();
        }

        public void OnProviderInitialized(string providerName)
        {
            AdProviderInitialized?.Invoke(providerName);
        }

        public void OnProviderAdLoaded(string providerName, AdType advertisingType)
        {
            AdLoaded?.Invoke(providerName, advertisingType);
        }

        public void OnProviderAdDisplayed(string providerName, AdType advertisingType)
        {
            AdDisplayed?.Invoke(providerName, advertisingType);

            if (advertisingType == AdType.Interstitial || advertisingType == AdType.RewardedVideo)
                ResetInterstitialDelayTime();
        }

        public void OnProviderAdClosed(string providerName, AdType advertisingType)
        {
            AdClosed?.Invoke(providerName, advertisingType);

            if (advertisingType == AdType.Interstitial || advertisingType == AdType.RewardedVideo)
                ResetInterstitialDelayTime();
        }

        private static double GetCurrentUnixTimestamp()
            => (DateTime.Now - DateTime.UnixEpoch).TotalSeconds;
    }
}
