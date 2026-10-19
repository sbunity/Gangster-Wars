using SBabchuk.Runtime.Progress;

namespace SBabchuk.EditorTools
{
    public interface IProgressEditSource
    {
        string Title { get; }
        bool CanDelete { get; }

        bool TryLoad(out PlayerProgress progress);
        void Save(PlayerProgress progress);
        void Delete();
    }
}
