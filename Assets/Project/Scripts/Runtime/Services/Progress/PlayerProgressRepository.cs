using SBabchuk.Runtime.Progress;
using SBabchuk.Runtime.Services.Contracts;

namespace SBabchuk.Runtime.Services
{
    public sealed class PlayerProgressRepository : IPlayerProgressRepository
    {
        private readonly IProgressStorage _storage;
        private readonly IPlayerProgressFactory _factory;
        private readonly IPlayerProgressNormalizer _normalizer;

        private PlayerProgress _progress;

        public PlayerProgressRepository(IProgressStorage storage, IPlayerProgressFactory factory, IPlayerProgressNormalizer normalizer)
        {
            _storage = storage;
            _factory = factory;
            _normalizer = normalizer;
        }

        public PlayerProgress Progress => _progress ??= LoadOrCreate();

        public void Save()
            => _storage.Save(Progress);

        private PlayerProgress LoadOrCreate()
        {
            if (_storage.TryLoad(out var progress))
            {
                if (_normalizer.Normalize(progress))
                    _storage.Save(progress);

                return progress;
            }

            progress = _factory.CreateDefault();
            _storage.Save(progress);
            return progress;
        }
    }
}
