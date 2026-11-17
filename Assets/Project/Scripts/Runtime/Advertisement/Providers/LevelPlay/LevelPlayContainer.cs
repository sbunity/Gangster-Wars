using System;
using UnityEngine;
using Zenject;

namespace SBabchuk.Runtime.Advertisement
{
    [Serializable]
    public sealed class LevelPlayContainer : AdsProviderContainer
    {
        [Header("Android")]
        [SerializeField] private string _androidAppKey;
        public string AndroidAppKey => _androidAppKey;

        [Space]
        [SerializeField] private string _androidBannerID;
        public string AndroidBannerID => _androidBannerID;

        [SerializeField] private string _androidInterstitialID;
        public string AndroidInterstitialID => _androidInterstitialID;

        [SerializeField] private string _androidRVID;
        public string AndroidRVID => _androidRVID;

        [Header("iOS")]
        [SerializeField] private string _iOSAppKey;
        public string IOSAppKey => _iOSAppKey;

        [Space]
        [SerializeField] private string _iOSBannerID;
        public string IOSBannerID => _iOSBannerID;

        [SerializeField] private string _iOSInterstitialID;
        public string IOSInterstitialID => _iOSInterstitialID;

        [SerializeField] private string _iOSRVID;
        public string IOSRVID => _iOSRVID;

        [Space]
        [SerializeField] private BannerPosition _bannerPosition;
        public BannerPosition BannerPosition => _bannerPosition;
        [SerializeField] private BannerPlacementType _bannerType;
        public BannerPlacementType BannerType => _bannerType;

        public enum BannerPlacementType
        {
            Banner = 0,
            Large = 1,
            Rectangle = 2,
            Leaderboard = 3
        }

        public override string ProviderName => "LevelPlay";
        public override AdProviderHandler CreateHandler(IInstantiator instantiator)
        {
#if MODULE_LEVELPLAY
            return instantiator.Instantiate<LevelPlayHandler>();
#else
            return null;
#endif
        }
    }
}
