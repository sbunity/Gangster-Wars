using SBabchuk.Runtime.Services.Contracts;

namespace SBabchuk.Runtime.Bootstrap
{
    public sealed class FirstSceneRouter : IFirstSceneRouter
    {
        private readonly ITutorialProgressService _tutorialProgress;
        private readonly IPlayerProgressService _progressService;
        private readonly BootstrapSettings _settings;

        public FirstSceneRouter(ITutorialProgressService tutorialProgress, IPlayerProgressService progressService, BootstrapSettings settings)
        {
            _tutorialProgress = tutorialProgress;
            _progressService = progressService;
            _settings = settings;
        }

        public Scene PrepareFirstScene()
        {
            if (_tutorialProgress.IsTutorialLevelCompleted)
                return _settings.FirstScene;

            _progressService.SetCurrentLevel(_tutorialProgress.TutorialLevelId);
            return Scene.GameScene;
        }
    }
}
