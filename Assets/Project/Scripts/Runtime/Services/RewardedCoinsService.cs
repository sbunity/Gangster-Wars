using SBabchuk.Runtime.Advertisement;
using SBabchuk.Runtime.Services.Contracts;

namespace SBabchuk.Runtime.Services
{
    public sealed class RewardedCoinsService : IRewardedCoinsService
    {
        private readonly IAdsService _adsService;
        private readonly IPlayerProgressService _progressService;
        private readonly AdPlacementConfig _config;

        private bool _isWatching;

        public RewardedCoinsService(IAdsService adsService, IPlayerProgressService progressService, IAssetProvider assetProvider)
        {
            _adsService = adsService;
            _progressService = progressService;
            _config = assetProvider.AdPlacementConfig;
        }

        public int Amount => _config.RewardedCoins;

        public bool CanWatch => !_isWatching && _adsService.IsRewardBasedVideoLoaded();

        public void Watch()
        {
            if (!CanWatch)
                return;

            _isWatching = true;
            _adsService.ShowRewardBasedVideo(OnAdFinished);
        }

        private void OnAdFinished(bool isRewarded)
        {
            if (!_isWatching)
                return;

            _isWatching = false;

            if (isRewarded)
                _progressService.AddCoins(Amount);
        }
    }
}
