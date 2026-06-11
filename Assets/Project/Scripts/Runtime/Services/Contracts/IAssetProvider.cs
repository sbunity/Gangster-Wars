using UnityEngine;

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
