using UnityEngine;
using SBabchuk.Runtime.Databases.BombStore;
using SBabchuk.Runtime.Databases.Bullets;
using SBabchuk.Runtime.Databases.DefenseStore;
using SBabchuk.Runtime.Databases.Enemies;
using SBabchuk.Runtime.Databases.Levels;
using SBabchuk.Runtime.Databases.MainPlayers;
using SBabchuk.Runtime.Databases.PlayerPrefs;
using SBabchuk.Runtime.Databases.WeaponStore;

namespace SBabchuk.Runtime.Services.Contracts
{
    public interface IAssetProvider
    {
        PlayerPrefsDatabase PlayerPrefsDatabase { get; }

        WeaponStoreDatabase WeaponStoreDatabase { get; }

        BombStoreDatabase BombStoreDatabase { get; }

        DefenseStoreDatabase DefenseStoreDatabase { get; }

        MainPlayerDatabase MainPlayerDatabase { get; }

        EnemyDatabase EnemyDatabase { get; }

        LevelDatabase LevelDatabase { get; }

        BulletDatabase BulletDatabase { get; }

        T LoadAsset<T>() where T : Object;
        GameObject LoadPrefab(NamesPool pool, int id);
    }
}
