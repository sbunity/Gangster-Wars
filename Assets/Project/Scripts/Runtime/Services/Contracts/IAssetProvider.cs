using UnityEngine;
using SBabchuk.Runtime.Advertisement;
using SBabchuk.Runtime.Databases.BombStore;
using SBabchuk.Runtime.Databases.Bullets;
using SBabchuk.Runtime.Databases.CarSkins;
using SBabchuk.Runtime.Databases.DefenseStore;
using SBabchuk.Runtime.Databases.Enemies;
using SBabchuk.Runtime.Databases.Levels;
using SBabchuk.Runtime.Databases.MainPlayers;
using SBabchuk.Runtime.Databases.WeaponStore;
using SBabchuk.Runtime.Progress;
using SBabchuk.Runtime.Purchasing;
using SBabchuk.Runtime.Tutorial;

namespace SBabchuk.Runtime.Services.Contracts
{
    public interface IAssetProvider
    {
        PlayerProgressDefaults PlayerProgressDefaults { get; }

        WeaponStoreDatabase WeaponStoreDatabase { get; }

        BombStoreDatabase BombStoreDatabase { get; }

        DefenseStoreDatabase DefenseStoreDatabase { get; }

        MainPlayerDatabase MainPlayerDatabase { get; }

        EnemyDatabase EnemyDatabase { get; }

        LevelDatabase LevelDatabase { get; }

        LevelBackgroundDatabase LevelBackgroundDatabase { get; }

        BulletDatabase BulletDatabase { get; }

        CarSkinDatabase CarSkinDatabase { get; }

        TutorialConfig TutorialConfig { get; }

        AdPlacementConfig AdPlacementConfig { get; }

        IAPRewardsConfig IAPRewardsConfig { get; }

        T LoadAsset<T>() where T : Object;
        GameObject LoadPrefab(NamesPool pool, int id);
    }
}
