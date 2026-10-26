namespace SBabchuk.Runtime.Services.Contracts
{
    public interface ITutorialProgressService
    {
        int TutorialLevelId { get; }

        bool IsTutorialLevelCompleted { get; }

        bool IsTutorialLevelActive { get; }

        bool IsLevelCompleted(int levelId);

        bool IsCompleted(TutorialStepId step);
        void Complete(TutorialStepId step);
    }
}
