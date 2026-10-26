using SBabchuk.Runtime.Tutorial.Steps;
using SBabchuk.Runtime.UI.Tutorial;
using UnityEngine;
using Zenject;

namespace SBabchuk.Runtime.Installers
{
    public sealed class MainSceneInstaller : MonoInstaller
    {
        [SerializeField] private TutorialOverlayView _tutorialView;

        public override void InstallBindings()
            => TutorialInstaller.Install(Container, _tutorialView, new[]
            {
                typeof(UpgradeWeaponTutorialStep),
                typeof(PlayTutorialStep),
            });
    }
}
