using SBabchuk.Runtime.Services.Contracts;
using SBabchuk.Runtime.Services.Models;
using Zenject;

namespace SBabchuk.Runtime.Tutorial.Steps
{
    public sealed class PlayTutorialStep : TapToTransitionTutorialStep
    {
        public PlayTutorialStep(
            SignalBus signalBus,
            ITutorialProgressService tutorialProgress,
            ITutorialTargetRegistry targets,
            ITutorialView view,
            IAssetProvider assetProvider)
            : base(signalBus, tutorialProgress, targets, view, assetProvider)
        {
        }

        public override TutorialStepId Id => TutorialStepId.Play;
        protected override TutorialTargetKey Target => TutorialTargetId.PlayButton;
        protected override string Caption => Config.PlayCaption;
        protected override Scene Destination => Scene.LevelSelect;
    }
}
