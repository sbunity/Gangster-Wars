using System.Threading;
using Cysharp.Threading.Tasks;
using SBabchuk.Runtime.Bootstrap;
using SBabchuk.Runtime.Services.Contracts;

namespace SBabchuk.Runtime.Advertisement
{
    public sealed class AdsInitializeStep : IBootstrapStep
    {
        private readonly IAdsService _adsService;
        private readonly AdsSettings _settings;

        public AdsInitializeStep(IAdsService adsService, AdsSettings settings)
        {
            _adsService = adsService;
            _settings = settings;
        }

        public UniTask ExecuteAsync(CancellationToken cancellationToken)
        {
            _adsService.Init(_settings);
            return UniTask.CompletedTask;
        }
    }
}
