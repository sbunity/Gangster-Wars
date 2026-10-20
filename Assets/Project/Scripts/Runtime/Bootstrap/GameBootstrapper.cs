using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace SBabchuk.Runtime.Bootstrap
{
    public sealed class GameBootstrapper : IGameBootstrapper
    {
        private readonly IReadOnlyList<IBootstrapStep> _steps;

        public GameBootstrapper(List<IBootstrapStep> steps)
        {
            _steps = steps;
        }

        public async UniTask RunAsync(CancellationToken cancellationToken)
        {
            foreach (var step in _steps)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                await step.ExecuteAsync(cancellationToken);
            }
        }
    }
}
