using SBabchuk.Runtime.Progress;
using SBabchuk.Runtime.Services.Contracts;

namespace SBabchuk.Runtime.Services
{
    public sealed class TutorialProgressService : ITutorialProgressService
    {
        private readonly IAssetProvider _assetProvider;
        private readonly IPlayerProgressRepository _repository;

        public TutorialProgressService(IAssetProvider assetProvider, IPlayerProgressRepository repository)
        {
            _assetProvider = assetProvider;
            _repository = repository;
        }

        private PlayerProgress Progress => _repository.Progress;

        public int TutorialLevelId => _assetProvider.TutorialConfig.TutorialLevelId;

        public bool IsTutorialLevelCompleted
            => IsLevelCompleted(TutorialLevelId);

        public bool IsTutorialLevelActive
            => Progress.LevelId == TutorialLevelId && !IsTutorialLevelCompleted;

        public bool IsLevelCompleted(int levelId)
            => Progress.GetLevelShortInfo(levelId)?.IsCompleted == mySwitch.On;

        public bool IsCompleted(TutorialStepId step)
            => Progress.HasCompletedTutorialStep((int)step);

        public void Complete(TutorialStepId step)
        {
            if (IsCompleted(step))
                return;

            Progress.CompletedTutorialSteps.Add((int)step);
            _repository.Save();
        }
    }
}
