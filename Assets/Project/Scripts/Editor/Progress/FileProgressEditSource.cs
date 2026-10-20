using System.Threading;
using Cysharp.Threading.Tasks;
using SBabchuk.Runtime.Progress;
using SBabchuk.Runtime.Services;

namespace SBabchuk.EditorTools
{
    public sealed class FileProgressEditSource : IProgressEditSource
    {
        private readonly FileProgressStorage _storage;

        public FileProgressEditSource(FileProgressStorage storage)
        {
            _storage = storage;
        }

        public string Title => "Save file";
        public bool CanDelete => _storage.Exists;

        public UniTask<PlayerProgress> LoadAsync(CancellationToken cancellationToken)
            => _storage.LoadAsync(cancellationToken);

        public UniTask SaveAsync(PlayerProgress progress, CancellationToken cancellationToken)
            => _storage.SaveAsync(progress, cancellationToken);

        public void Delete()
            => _storage.Delete();
    }
}
