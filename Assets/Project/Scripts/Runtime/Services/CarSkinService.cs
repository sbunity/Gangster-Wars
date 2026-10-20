using System.Collections.Generic;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Databases.CarSkins;
using SBabchuk.Runtime.Progress;
using SBabchuk.Runtime.Services.Contracts;
using Zenject;

namespace SBabchuk.Runtime.Services
{
    public sealed class CarSkinService : ICarSkinService
    {
        private readonly IAssetProvider _assetProvider;
        private readonly IPlayerProgressRepository _repository;
        private readonly IPlayerProgressService _progressService;
        private readonly SignalBus _signalBus;

        public CarSkinService(
            IAssetProvider assetProvider,
            IPlayerProgressRepository repository,
            IPlayerProgressService progressService,
            SignalBus signalBus)
        {
            _assetProvider = assetProvider;
            _repository = repository;
            _progressService = progressService;
            _signalBus = signalBus;
        }

        private PlayerProgress Progress => _repository.Progress;
        private CarSkinDatabase Database => _assetProvider.CarSkinDatabase;

        public IReadOnlyList<CarSkin> Skins => Database.Skins;
        public int SelectedSkinId => Progress.SelectedCarSkinId;
        public CarSkin SelectedSkin => Database.GetSkin(SelectedSkinId);

        public bool IsOwned(int id)
            => Progress.HasCarSkin(id);

        public bool CanBuy(int id)
        {
            var skin = Database.GetSkin(id);
            return skin != null && !IsOwned(id) && _progressService.CanBuy(skin.Price);
        }

        public bool TryBuy(int id)
        {
            if (!CanBuy(id))
                return false;

            Progress.OwnedCarSkinIds.Add(id);
            Progress.SelectedCarSkinId = id;
            _progressService.AddCoins(-Database.GetSkin(id).Price);
            FireSkinChanged();
            return true;
        }

        public bool TrySelect(int id)
        {
            if (!IsOwned(id) || id == SelectedSkinId)
                return false;

            Progress.SelectedCarSkinId = id;
            _repository.Save();
            FireSkinChanged();
            return true;
        }

        private void FireSkinChanged()
            => _signalBus.Fire(new CarSkinChangedSignal(SelectedSkinId));
    }
}
