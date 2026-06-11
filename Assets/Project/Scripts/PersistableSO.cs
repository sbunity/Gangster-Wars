using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace SBabchuk
{
    public class PersistableSO : MonoBehaviour
    {
        [SerializeField, FormerlySerializedAs("objectsToPersist")]
        private List<ScriptableObject> _objectsToPersist;

        [SerializeField, FormerlySerializedAs("PlayerPrefs")]
        private PlayerPrefsDatabase _playerPrefs;

        [SerializeField, FormerlySerializedAs("WeaponStore")]
        private WeaponStoreDatabase _weaponStore;

        [SerializeField, FormerlySerializedAs("BombStore")]
        private BombStoreDatabase _bombStore;

        [SerializeField, FormerlySerializedAs("DefenceStore")]
        private DefenseStoreDatabase _defenceStore;

        [SerializeField, FormerlySerializedAs("PlayerStore")]
        private MainPlayerDatabase _playerStore;

        [SerializeField, FormerlySerializedAs("Enemy")]
        private EnemyDatabase _enemy;

        [SerializeField, FormerlySerializedAs("Level")]
        private LevelDatabase _level;

        [SerializeField, FormerlySerializedAs("Bullet")]
        private BulletDatabase _bullet;
    }
}
