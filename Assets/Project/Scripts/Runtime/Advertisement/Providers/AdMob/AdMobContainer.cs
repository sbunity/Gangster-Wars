using System;
using UnityEngine;
using Zenject;

namespace SBabchuk.Runtime.Advertisement
{
    [Serializable]
    public sealed class AdMobContainer : AdsProviderContainer
    {
        public const string AndroidBannerTestId = "ca-app-pub-3940256099942544/6300978111";
        public const string IOSBannerTestId = "ca-app-pub-3940256099942544/2934735716";
        public const string AndroidInterstitialTestId = "ca-app-pub-3940256099942544/1033173712";
        public const string IOSInterstitialTestId = "ca-app-pub-3940256099942544/4411468910";
        public const string AndroidRewardedVideoTestId = "ca-app-pub-3940256099942544/5224354917";
        public const string IOSRewardedVideoTestId = "ca-app-pub-3940256099942544/1712485313";
        public const string AndroidOpenTestId = "ca-app-pub-3940256099942544/9257395921";
        public const string IOSOpenTestId = "ca-app-pub-3940256099942544/5575463023";

        [SerializeField] private string _androidAppId;
        [SerializeField] private string _iosAppId;

        [SerializeField] private string _androidBannerID = AndroidBannerTestId;
        public string AndroidBannerID => _androidBannerID;
        [SerializeField] private string _iOSBannerID = IOSBannerTestId;
        public string IOSBannerID => _iOSBannerID;

        [SerializeField] private string _androidInterstitialID = AndroidInterstitialTestId;
        public string AndroidInterstitialID => _androidInterstitialID;
        [SerializeField] private string _iOSInterstitialID = IOSInterstitialTestId;
        public string IOSInterstitialID => _iOSInterstitialID;

        [SerializeField] private string _androidRewardedVideoID = AndroidRewardedVideoTestId;
        public string AndroidRewardedVideoID => _androidRewardedVideoID;
        [SerializeField] private string _iOSRewardedVideoID = IOSRewardedVideoTestId;
        public string IOSRewardedVideoID => _iOSRewardedVideoID;

        [SerializeField] private bool _useAppOpenAd;
        public bool UseAppOpenAd => _useAppOpenAd;

        [SerializeField] private string _androidAppOpenAdID = AndroidOpenTestId;
        public string AndroidAppOpenAdID => _androidAppOpenAdID;
        [SerializeField] private string _iOSAppOpenAdID = IOSOpenTestId;
        public string IOSAppOpenAdID => _iOSAppOpenAdID;

        [Space]
        [SerializeField] private int _appOpenAdExpirationHoursTime = 4;
        public int AppOpenAdExpirationHoursTime => _appOpenAdExpirationHoursTime;

        [SerializeField] private BannerPlacementType _bannerType = BannerPlacementType.Banner;
        public BannerPlacementType BannerType => _bannerType;

        [SerializeField] private BannerPosition _bannerPosition = BannerPosition.Bottom;
        public BannerPosition BannerPosition => _bannerPosition;

        public enum BannerPlacementType
        {
            Banner = 0,
            MediumRectangle = 1,
            IABBanner = 2,
            Leaderboard = 3,
        }

        public override string ProviderName => "AdMob";
        public override AdProviderHandler CreateHandler(IInstantiator instantiator)
        {
#if MODULE_ADMOB
            return instantiator.Instantiate<AdMobHandler>();
#else
            return null;
#endif
        }
    }
}
