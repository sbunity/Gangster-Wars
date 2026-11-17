using SBabchuk.Runtime.Advertisement;
using SBabchuk.Runtime.Bootstrap;
using SBabchuk.Runtime.Purchasing;
using SBabchuk.Runtime.Services;
using UnityEngine;
using Zenject;

namespace SBabchuk.Runtime.Installers
{
    public sealed class InitSceneInstaller : MonoInstaller
    {
        [SerializeField]
        private BootstrapSettings _settings = new();

        [SerializeField]
        private ATTSettings _attSettings = new();

        [SerializeField]
        private UMPSettings _umpSettings = new();

        [SerializeField]
        private AdsSettings _adsSettings;

        [SerializeField]
        private IAPSettings _iapSettings;

        public override void InstallBindings()
        {
            BindBootstrapSteps();
            Container.Bind<IGameBootstrapper>().To<GameBootstrapper>().AsSingle();
            Container.Bind<IFirstSceneRouter>().To<FirstSceneRouter>().AsSingle().WithArguments(_settings);
            Container.BindInterfacesTo<InitSceneEntryPoint>().AsSingle().WithArguments(_settings);
        }

        private void BindBootstrapSteps()
        {
            Container.Bind<IBootstrapStep>().To<LegacySaveFilesCleanup>().AsSingle();
            Container.Bind<IBootstrapStep>().To<PlayerProgressLoadStep>().AsSingle();
            Container.Bind<IBootstrapStep>().To<ATTConsentStep>().AsSingle().WithArguments(_attSettings);
            Container.Bind<IBootstrapStep>().To<UMPConsentStep>().AsSingle().WithArguments(_umpSettings);
            Container.Bind<IBootstrapStep>().To<AdsInitializeStep>().AsSingle().WithArguments(_adsSettings);
            Container.Bind<IBootstrapStep>().To<IAPInitializeStep>().AsSingle().WithArguments(_iapSettings);
        }
    }
}
