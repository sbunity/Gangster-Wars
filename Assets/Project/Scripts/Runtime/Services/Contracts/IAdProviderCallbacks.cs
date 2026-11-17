using System;
using SBabchuk.Runtime.Advertisement;

namespace SBabchuk.Runtime.Services.Contracts
{
    public interface IAdProviderCallbacks
    {
        void CallEventInMainThread(Action callback);
        void OnProviderInitialized(string providerName);
        void OnProviderAdLoaded(string providerName, AdType advertisingType);
        void OnProviderAdDisplayed(string providerName, AdType advertisingType);
        void OnProviderAdClosed(string providerName, AdType advertisingType);
        void ExecuteInterstitialCallback(bool result);
        void ExecuteRewardVideoCallback(bool result);
    }
}
