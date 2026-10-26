using SBabchuk.Runtime.Services.Models;

namespace SBabchuk.Runtime.Services.Contracts
{
    public interface ITutorialView
    {
        void Show(TutorialHint hint);
        void Hide();
    }
}
