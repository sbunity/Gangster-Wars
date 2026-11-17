using SBabchuk.Runtime.Advertisement;
using SBabchuk.Runtime.Services.Contracts;

namespace SBabchuk.Runtime.Services
{
    public sealed class ConsentData : IConsentData
    {
        public bool IsConsentGiven { get; set; }
        public AuthorizationTrackingStatus ATTStatus { get; set; } = AuthorizationTrackingStatus.NOT_DETERMINED;
    }
}
