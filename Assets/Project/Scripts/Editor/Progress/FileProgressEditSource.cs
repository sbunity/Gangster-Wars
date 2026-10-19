using SBabchuk.Runtime.Progress;
using SBabchuk.Runtime.Services.Contracts;

namespace SBabchuk.EditorTools
{
    public sealed class FileProgressEditSource : IProgressEditSource
    {
        private readonly IProgressStorage _storage;

        public FileProgressEditSource(IProgressStorage storage)
        {
            _storage = storage;
        }

        public string Title => "Save file";
        public bool CanDelete => _storage.Exists;

        public bool TryLoad(out PlayerProgress progress)
            => _storage.TryLoad(out progress);

        public void Save(PlayerProgress progress)
            => _storage.Save(progress);

        public void Delete()
            => _storage.Delete();
    }
}
