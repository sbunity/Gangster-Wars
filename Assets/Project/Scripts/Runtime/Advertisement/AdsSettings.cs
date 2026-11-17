using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SBabchuk.Runtime.Advertisement
{
    [HelpURL("https://www.notion.so/wmelongames/Advertisement-221053e32d4047bb880275027daba9f0?pvs=4")]
    [CreateAssetMenu(menuName = "Advertisement/Create AdsSettings", fileName = "AdsSettings")]
    public sealed class AdsSettings : ScriptableObject
    {
        [SerializeField] private string _activeProvider = "Dummy";
        public string ActiveProvider => _activeProvider;

        [SerializeField] private bool _debugMode;
        public bool DebugMode => _debugMode;

        [SerializeField] private List<string> _testDevices;
        public List<string> TestDevices => _testDevices;

        [SerializeField] private bool _loadAdsOnStart = true;
        public bool LoadAdsOnStart => _loadAdsOnStart;

        [SerializeField] private bool _bannerEnabled = true;
        public bool BannerEnabled => _bannerEnabled;

        [SerializeField] private bool _interstitialEnabled = true;
        public bool InterstitialEnabled => _interstitialEnabled;

        [SerializeField] private bool _rewardedVideoEnabled = true;
        public bool RewardedVideoEnabled => _rewardedVideoEnabled;

        [Tooltip("Delay in seconds before interstitial appearings on first game launch.")]
        [SerializeField] private float _interstitialFirstStartDelay = 40f;
        public float InterstitialFirstStartDelay => _interstitialFirstStartDelay;

        [Tooltip("Delay in seconds before interstitial appearings.")]
        [SerializeField] private float _interstitialStartDelay = 40f;
        public float InterstitialStartDelay => _interstitialStartDelay;

        [Tooltip("Delay in seconds between interstitial appearings.")]
        [SerializeField] private float _interstitialShowingDelay = 30f;
        public float InterstitialShowingDelay => _interstitialShowingDelay;

        [SerializeField] private float _loadingAdDuration;
        public float LoadingAdDuration => _loadingAdDuration;

        [SerializeField] private string _loadingMessage = "Ad is loading..";
        public string LoadingMessage => _loadingMessage;

        [SerializeReference]
        private List<AdsProviderContainer> _providerContainers = new();

        [SerializeField] private string _privacyLink = "";
        public string PrivacyLink => _privacyLink;

        [SerializeField] private string _termsOfUseLink = "";
        public string TermsOfUseLink => _termsOfUseLink;

        public T GetContainer<T>() where T : AdsProviderContainer
            => _providerContainers.OfType<T>().FirstOrDefault();

        public AdsProviderContainer GetContainer(string providerName)
            => _providerContainers.FirstOrDefault(container => container != null && container.ProviderName == providerName);

        public bool HasContainer(Type containerType)
            => _providerContainers.Any(container => container != null && container.GetType() == containerType);

        public void AddContainer(AdsProviderContainer container)
        {
            _providerContainers.Add(container);
        }

        public void DisableBanner() => _bannerEnabled = false;
        public void DisableInterstitial() => _interstitialEnabled = false;
        public void DisableRewardedVideo() => _rewardedVideoEnabled = false;
    }
}
