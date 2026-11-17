using System.Threading;
using Cysharp.Threading.Tasks;
using SBabchuk.Runtime.Bootstrap;
using SBabchuk.Runtime.Services.Contracts;

namespace SBabchuk.Runtime.Purchasing
{
    public sealed class IAPInitializeStep : IBootstrapStep
    {
        private readonly IIAPService _iapService;
        private readonly IAPSettings _settings;

        public IAPInitializeStep(IIAPService iapService, IAPSettings settings)
        {
            _iapService = iapService;
            _settings = settings;
        }

        public UniTask ExecuteAsync(CancellationToken cancellationToken)
        {
            _iapService.Init(_settings);
            return UniTask.CompletedTask;
        }
    }
}
