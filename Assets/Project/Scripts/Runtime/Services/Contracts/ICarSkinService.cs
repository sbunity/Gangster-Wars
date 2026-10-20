using System.Collections.Generic;
using SBabchuk.Runtime.Databases.CarSkins;

namespace SBabchuk.Runtime.Services.Contracts
{
    public interface ICarSkinService
    {
        IReadOnlyList<CarSkin> Skins { get; }

        int SelectedSkinId { get; }

        CarSkin SelectedSkin { get; }

        bool IsOwned(int id);
        bool CanBuy(int id);
        bool TryBuy(int id);
        bool TrySelect(int id);
    }
}
