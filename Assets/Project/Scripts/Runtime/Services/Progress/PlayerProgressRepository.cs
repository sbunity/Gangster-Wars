using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Progress;
using SBabchuk.Runtime.Services.Contracts;

namespace SBabchuk.Runtime.Services
{
    public sealed class PlayerProgressRepository : IPlayerProgressRepository
    {
        private const string NotLoadedMessage = "Player progress is not loaded yet. It is loaded by the Init scene.";

        private readonly IProgressStorage _storage;
        private readonly IPlayerProgressFactory _factory;
        private readonly IPlayerProgressNormalizer _normalizer;
        private readonly CoalescingTaskRunner _saveRunner;

        private PlayerProgress _progress;

        public PlayerProgressRepository(IProgressStorage storage, IPlayerProgressFactory factory, IPlayerProgressNormalizer normalizer)
        {
            _storage = storage;
            _factory = factory;
            _normalizer = normalizer;
            _saveRunner = new CoalescingTaskRunner(SaveCurrentAsync);
        }

        public bool IsLoaded => _progress != null;

        public PlayerProgress Progress => _progress ?? throw new InvalidOperationException(NotLoadedMessage);

        public async UniTask LoadAsync(CancellationToken cancellationToken)
            => _progress = await LoadOrCreateAsync(cancellationToken);

        public void Save()
        {
            if (!IsLoaded)
                throw new InvalidOperationException(NotLoadedMessage);

            _saveRunner.Run();
        }

        private UniTask SaveCurrentAsync()
            => _storage.SaveAsync(_progress, CancellationToken.None);

        private async UniTask<PlayerProgress> LoadOrCreateAsync(CancellationToken cancellationToken)
        {
            var progress = await _storage.LoadAsync(cancellationToken);
            if (progress != null)
            {
                if (_normalizer.Normalize(progress))
                    await _storage.SaveAsync(progress, cancellationToken);

                return progress;
            }

            progress = _factory.CreateDefault();
            await _storage.SaveAsync(progress, cancellationToken);
            return progress;
        }
    }
}
