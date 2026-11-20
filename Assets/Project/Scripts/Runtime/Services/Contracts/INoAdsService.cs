namespace SBabchuk.Runtime.Services.Contracts
{
    public interface INoAdsService
    {
        bool AdsRemoved { get; }
        void RemoveAds();
    }
}
