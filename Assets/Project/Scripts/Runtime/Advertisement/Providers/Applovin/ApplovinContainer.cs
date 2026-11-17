using System;
using UnityEngine;
using Zenject;

namespace SBabchuk.Runtime.Advertisement
{
    [Serializable]
    public sealed class ApplovinContainer : AdsProviderContainer
    {
        [Header("Banner ID")]
        [SerializeField] private string _androidBannerID = "banner";
        public string AndroidBannerID => _androidBannerID;
        [SerializeField] private string _iOSBannerID = "banner";
        public string IOSBannerID => _iOSBannerID;

        [Header("Interstitial ID")]
        [SerializeField] private string _androidInterstitialID = "video";
        public string AndroidInterstitialID => _androidInterstitialID;
        [SerializeField] private string _iOSInterstitialID = "video";
        public string IOSInterstitialID => _iOSInterstitialID;

        [Header("Rewarded Video ID")]
        [SerializeField] private string _androidRewardedVideoID = "rewardedVideo";
        public string AndroidRewardedVideoID => _androidRewardedVideoID;
        [SerializeField] private string _iOSRewardedVideoID = "rewardedVideo";
        public string IOSRewardedVideoID => _iOSRewardedVideoID;

        [Space]
        [SerializeField] private BannerPositionAL _bannerPosition;
        public BannerPositionAL BannerPosition => _bannerPosition;

        [SerializeField] private bool _useAdaptiveBanner = true;
        public bool UseAdaptiveBanner => _useAdaptiveBanner;

        [SerializeField] private Color _bannerBackgroundColor = Color.white;
        public Color BannerBackgroundColor => _bannerBackgroundColor;

        public enum BannerPositionAL
        {
            TopLeft,
            TopCenter,
            TopRight,
            Centered,
            CenterLeft,
            CenterRight,
            BottomLeft,
            BottomCenter,
            BottomRight
        }

        public override string ProviderName => "Applovin";
        public override AdProviderHandler CreateHandler(IInstantiator instantiator)
        {
#if MODULE_APPLOVIN
            return instantiator.Instantiate<ApplovinHandler>();
#else
            return null;
#endif
        }
    }
}
