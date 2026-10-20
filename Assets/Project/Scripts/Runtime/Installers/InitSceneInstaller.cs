using SBabchuk.Runtime.Bootstrap;
using SBabchuk.Runtime.Services;
using UnityEngine;
using Zenject;

namespace SBabchuk.Runtime.Installers
{
    public sealed class InitSceneInstaller : MonoInstaller
    {
        [SerializeField]
        private BootstrapSettings _settings = new();

        public override void InstallBindings()
        {
            BindBootstrapSteps();
            Container.Bind<IGameBootstrapper>().To<GameBootstrapper>().AsSingle();
            Container.BindInterfacesTo<InitSceneEntryPoint>().AsSingle().WithArguments(_settings);
        }

        private void BindBootstrapSteps()
        {
            Container.Bind<IBootstrapStep>().To<LegacySaveFilesCleanup>().AsSingle();
            Container.Bind<IBootstrapStep>().To<PlayerProgressLoadStep>().AsSingle();
        }
    }
}
