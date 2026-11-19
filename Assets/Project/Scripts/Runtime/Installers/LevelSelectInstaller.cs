using SBabchuk.Runtime.Tutorial.Steps;
using SBabchuk.Runtime.UI.Intro;
using SBabchuk.Runtime.UI.Tutorial;
using UnityEngine;
using Zenject;

namespace SBabchuk.Runtime.Installers
{
    public sealed class LevelSelectInstaller : MonoInstaller
    {
        [SerializeField] private TutorialOverlayView _tutorialView;
        [SerializeField] private UIIntroView _uiIntro;

        public override void InstallBindings()
        {
            SceneIntroInstaller.Install(Container, _uiIntro);

            TutorialInstaller.Install(Container, _tutorialView, new[]
            {
                typeof(SelectLevelTutorialStep),
            });
        }
    }
}
