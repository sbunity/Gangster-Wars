using SBabchuk.Runtime.Services.Contracts;
using SBabchuk.Runtime.Tutorial.Steps;
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

        public override void InstallBindings()
        {
            Container.Bind<IStoreTabs>().FromInstance(_storeTabs).AsSingle();

            TutorialInstaller.Install(Container, _tutorialView, new[]
            {
                typeof(UpgradeWeaponTutorialStep),
                typeof(PlayTutorialStep),
                typeof(StoreTourTutorialStep),
            });
        }
    }
}
