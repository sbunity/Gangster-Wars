using System;
using SBabchuk.Runtime.Advertisement;

namespace SBabchuk.Runtime.Services.Contracts
{
    public interface IAdsService
    {
        AdsSettings Settings { get; }
        bool DebugMode { get; }
        bool IsBannerActive { get; }
        float BannerHeight { get; }

        event Action ForcedAdDisabled;
        event Action<string> AdProviderInitialized;
        event Action<string, AdType> AdLoaded;
        event Action<string, AdType> AdDisplayed;
        event Action<string, AdType> AdClosed;
        event Func<bool> InterstitialConditions;

        void Init(AdsSettings adsSettings);
        void TryToLoadFirstAds();
        void ShowErrorMessage();
        bool IsProviderActive();
        bool IsProviderInitialized();
        AdProviderHandler GetActiveProvider();

        bool IsInterstitialLoaded();
        void RequestInterstitial();
        void ShowInterstitial(AdProviderHandler.AdvertisementCallback callback, bool ignoreConditions = false);
        void SetInterstitialDelayTime(float time);
        void ResetInterstitialDelayTime();
        bool CheckExtraInterstitialCondition();

        bool IsRewardBasedVideoLoaded();
        void RequestRewardBasedVideo();
        void ShowRewardBasedVideo(AdProviderHandler.AdvertisementCallback callback, bool showErrorMessage = true);

        void ShowBanner();
        void DestroyBanner();
        void HideBanner();
        void EnableBanner();
        void DisableBanner();
        void ResetBannerHeight();
        void SetBannerHeight(float height);
        float GetBannerHeight();

        bool IsForcedAdEnabled();
        void DisableForcedAdForever();
        void DisableForcedAd(int durationSeconds);
    }
}
