using SBabchuk.Runtime.Progress;
using SBabchuk.Runtime.Services.Contracts;

namespace SBabchuk.Runtime.Services
{
    public sealed class PlayerProgressFactory : IPlayerProgressFactory
    {
        private readonly IAssetProvider _assetProvider;
        private readonly IPlayerProgressNormalizer _normalizer;

        public PlayerProgressFactory(IAssetProvider assetProvider, IPlayerProgressNormalizer normalizer)
        {
            _assetProvider = assetProvider;
            _normalizer = normalizer;
        }

        public PlayerProgress CreateDefault()
        {
            var defaults = _assetProvider.PlayerProgressDefaults;
            var progress = new PlayerProgress
            {
                Music = ToSwitch(defaults.MusicEnabled),
                Sound = ToSwitch(defaults.SoundEnabled),
                Vibration = ToSwitch(defaults.VibrationEnabled),
                Coin = defaults.Coins,
                LevelId = defaults.LevelId,
                SelectedWeaponId = defaults.StarterWeaponId,
                SelectedGrenadeId = defaults.SelectedGrenadeId,
                SelectedDefenceId = defaults.StarterDefenceId
            };

            _normalizer.Normalize(progress);
            return progress;
        }

        private static mySwitch ToSwitch(bool value)
            => value ? mySwitch.On : mySwitch.Off;
    }
}
