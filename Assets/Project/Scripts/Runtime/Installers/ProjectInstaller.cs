using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Audio;
using SBabchuk.Runtime.Factories;
using SBabchuk.Runtime.Services;
using SBabchuk.Runtime.Services.Contracts;
using SBabchuk.Runtime.UI;
using SBabchuk.Runtime.Vibration;
using UnityEngine;
using Zenject;

namespace SBabchuk.Runtime.Installers
{
    public sealed class ProjectInstaller : MonoInstaller
    {
        [SerializeField]
        private LoadingScreenView _loadingScreenPrefab;

        [SerializeField]
        private UISoundsConfig _uiSounds;

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
            Container.Bind<ILoadingScreenFactory>().To<LoadingScreenFactory>().AsSingle()
                .WithArguments(_loadingScreenPrefab);
            Container.Bind<ISceneTransitionService>().To<SceneTransitionService>().AsSingle();
            Container.BindInterfacesTo<LevelService>().AsSingle();
            Container.Bind<IDamageService>().To<DamageService>().AsSingle();
            Container.Bind<ICombatService>().To<CombatService>().AsSingle();
            Container.Bind<IEnemyDiscoveryService>().To<EnemyDiscoveryService>().AsSingle();
            Container.Bind<IEnemyInfoService>().To<EnemyInfoService>().AsSingle();
            Container.Bind<IWaveSkipRewardService>().To<WaveSkipRewardService>().AsSingle();
            Container.Bind<IBonusDropService>().To<BonusDropService>().AsSingle();
            Container.Bind<IBonusRewardService>().To<BonusRewardService>().AsSingle();
            Container.Bind<IBonusCollectTargetRegistry>().To<BonusCollectTargetRegistry>().AsSingle();
            Container.Bind<IStoreItemStatsService>().To<StoreItemStatsService>().AsSingle();
            Container.Bind<IMainPlayerStatsService>().To<MainPlayerStatsService>().AsSingle();
            Container.Bind<IAudioSettingsService>().To<AudioSettingsService>().AsSingle();
            Container.BindInterfacesTo<AudioService>().AsSingle();
            Container.BindInterfacesTo<MusicService>().AsSingle();
            Container.Bind<UISoundsConfig>().FromInstance(_uiSounds).AsSingle();
            Container.Bind<IInputService>().To<InputService>().AsSingle();
            BindVibration();
        }

        private void BindVibration()
        {
            Container.Bind<IVibrationSettingsService>().To<VibrationSettingsService>().AsSingle();
            Container.Bind<IVibrationService>().To<VibrationService>().AsSingle();
#if UNITY_ANDROID && !UNITY_EDITOR
            Container.BindInterfacesTo<AndroidHapticDevice>().AsSingle();
#elif UNITY_IOS && !UNITY_EDITOR
            Container.Bind<IHapticDevice>().To<IosHapticDevice>().AsSingle();
#else
            Container.Bind<IHapticDevice>().To<NullHapticDevice>().AsSingle();
#endif
        }

        private void DeclareSignals()
        {
            Container.DeclareSignal<EnemyDiedSignal>().OptionalSubscriber();
            Container.DeclareSignal<NewEnemyDiscoveredSignal>().OptionalSubscriber();
            Container.DeclareSignal<NewEnemyNotificationSelectedSignal>().OptionalSubscriber();
            Container.DeclareSignal<BonusPoppedSignal>().OptionalSubscriber();
            Container.DeclareSignal<GameFinishedSignal>().OptionalSubscriber();
            Container.DeclareSignal<CoinsChangedSignal>().OptionalSubscriber();
            Container.DeclareSignal<CoinFlightRequestedSignal>().OptionalSubscriber();
            Container.DeclareSignal<CoinFlightFromScreenRequestedSignal>().OptionalSubscriber();
            Container.DeclareSignal<ProgressUpgradedSignal>().OptionalSubscriber();
            Container.DeclareSignal<WeaponAmmoChangedSignal>().OptionalSubscriber();
            Container.DeclareSignal<LeaderMagazineInitializedSignal>().OptionalSubscriber();
            Container.DeclareSignal<LeaderPatronsChangedSignal>().OptionalSubscriber();
            Container.DeclareSignal<WaveCountdownStartedSignal>().OptionalSubscriber();
            Container.DeclareSignal<WaveCountdownSkippedSignal>().OptionalSubscriber();
            Container.DeclareSignal<BarricadeHealthChangedSignal>().OptionalSubscriber();
            Container.DeclareSignal<BarricadeDamagedSignal>().OptionalSubscriber();
            Container.DeclareSignal<GrenadeDamageSignal>().OptionalSubscriber();
            Container.DeclareSignal<AudioSettingsChangedSignal>().OptionalSubscriber();
            Container.DeclareSignal<UIPanelReplaceSignal>().OptionalSubscriber();
        }
    }
}
