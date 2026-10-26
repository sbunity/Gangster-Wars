using SBabchuk.Runtime.Services.Contracts;
using SBabchuk.Runtime.Services.Models;
using Zenject;

namespace SBabchuk.Runtime.Tutorial.Steps
{
    public sealed class SelectLevelTutorialStep : TapToTransitionTutorialStep
    {
        public SelectLevelTutorialStep(
            SignalBus signalBus,
            ITutorialProgressService tutorialProgress,
            ITutorialTargetRegistry targets,
            ITutorialView view,
            IAssetProvider assetProvider)
            : base(signalBus, tutorialProgress, targets, view, assetProvider)
        {
        }

        public override TutorialStepId Id => TutorialStepId.SelectLevel;
        protected override TutorialTargetKey Target => new(TutorialTargetId.LevelButton, Config.GrenadeLevelId);
        protected override string Caption => Config.SelectLevelCaption;
        protected override Scene Destination => Scene.GameScene;
    }
}
