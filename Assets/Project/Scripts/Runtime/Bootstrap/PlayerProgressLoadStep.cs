using System.Threading;
using Cysharp.Threading.Tasks;
using SBabchuk.Runtime.Services.Contracts;

namespace SBabchuk.Runtime.Bootstrap
{
    public sealed class PlayerProgressLoadStep : IBootstrapStep
    {
        private readonly IPlayerProgressRepository _repository;

        public PlayerProgressLoadStep(IPlayerProgressRepository repository)
        {
            _repository = repository;
        }

        public UniTask ExecuteAsync(CancellationToken cancellationToken)
            => _repository.LoadAsync(cancellationToken);
    }
}
