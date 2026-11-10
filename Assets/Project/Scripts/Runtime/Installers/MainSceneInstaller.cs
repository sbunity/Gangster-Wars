using SBabchuk.Runtime.Services.Contracts;
using SBabchuk.Runtime.Tutorial.Steps;
using SBabchuk.Runtime.UI.Intro;
using SBabchuk.Runtime.UI.Tutorial;
using SBabchuk.Runtime.UI.WeaponStore;
using UnityEngine;
using Zenject;

namespace SBabchuk.Runtime.Installers
{
    public sealed class MainSceneInstaller : MonoInstaller
    {
        [SerializeField] private TutorialOverlayView _tutorialView;
        [SerializeField] private StoreTabsView _storeTabs;
        [SerializeField] private UIIntroView _uiIntro;

        public override void InstallBindings()
        {
            Container.Bind<IStoreTabs>().FromInstance(_storeTabs).AsSingle();
            SceneIntroInstaller.Install(Container, _uiIntro);

            TutorialInstaller.Install(Container, _tutorialView, new[]
            {
                typeof(UpgradeWeaponTutorialStep),
                typeof(PlayTutorialStep),
                typeof(StoreTourTutorialStep),
            });
        }
    }
}
