using SBabchuk.Runtime.Advertisement;

namespace SBabchuk.Runtime.Services.Contracts
{
    public interface IConsentData
    {
        bool IsConsentGiven { get; set; }
        AuthorizationTrackingStatus ATTStatus { get; set; }
    }
}
