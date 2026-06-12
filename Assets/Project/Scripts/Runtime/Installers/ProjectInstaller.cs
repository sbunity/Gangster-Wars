using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Services;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using Zenject;

namespace SBabchuk.Runtime.Installers
{
    public sealed class ProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);
            BindServices();
            DeclareSignals();
        }

        private void BindServices()
        {
            Container.Bind<IPoolAssetResolver>().To<PoolAssetResolver>().AsSingle();
            Container.Bind<IAssetProvider>().To<AssetProvider>().AsSingle();
            Container.BindInterfacesTo<SaveService>().AsSingle();
            Container.BindInterfacesTo<PlayerProgressService>().AsSingle();
            Container.Bind<ISceneLoaderService>().To<SceneLoaderService>().AsSingle();
            Container.BindInterfacesTo<LevelService>().AsSingle();
            Container.Bind<IDamageService>().To<DamageService>().AsSingle();
            Container.Bind<ICombatService>().To<CombatService>().AsSingle();
            Container.Bind<IWaveSkipRewardService>().To<WaveSkipRewardService>().AsSingle();
            Container.Bind<IBonusDropService>().To<BonusDropService>().AsSingle();
            Container.Bind<IAudioSettingsService>().To<AudioSettingsService>().AsSingle();
            Container.Bind<IInputService>().To<InputService>().AsSingle();
        }

        private void DeclareSignals()
        {
            Container.DeclareSignal<EnemyDiedSignal>().OptionalSubscriber();
            Container.DeclareSignal<BonusPoppedSignal>().OptionalSubscriber();
            Container.DeclareSignal<GameFinishedSignal>().OptionalSubscriber();
            Container.DeclareSignal<CoinsChangedSignal>().OptionalSubscriber();
            Container.DeclareSignal<ProgressUpgradedSignal>().OptionalSubscriber();
            Container.DeclareSignal<WeaponAmmoChangedSignal>().OptionalSubscriber();
            Container.DeclareSignal<LeaderMagazineInitializedSignal>().OptionalSubscriber();
            Container.DeclareSignal<LeaderPatronsChangedSignal>().OptionalSubscriber();
            Container.DeclareSignal<BarricadeHealthChangedSignal>().OptionalSubscriber();
            Container.DeclareSignal<GrenadeDamageSignal>().OptionalSubscriber();
            Container.DeclareSignal<AudioSettingsChangedSignal>().OptionalSubscriber();
            Container.DeclareSignal<UIPanelReplaceSignal>().OptionalSubscriber();
        }
    }
}
