using System.Collections.Generic;
using UnityEngine;

namespace SBabchuk
{
    public class PersistableSO : MonoBehaviour
    {
        [Header("База, що змынюэться ы ъъ потрыбно пыдгружати")]
        public List<ScriptableObject> objectsToPersist;

        [Header("База даних PlayerPrefs")]
        public PlayerPrefsDatabase PlayerPrefs;

        [Header("База даних зброї")]
        public WeaponStoreDatabase WeaponStore;

        [Header("База даних гранат")]
        public BombStoreDatabase BombStore;

        [Header("База даних перепон")]
        public DefenseStoreDatabase DefenceStore;

        [Header("База даних персонажів")]
        public MainPlayerDatabase PlayerStore;

        [Header("База даних ворогів")]
        public EnemyDatabase Enemy;

        [Header("База даних лквклів")]
        public LevelDatabase Level;

        [Header("База даних пуль")]
        public BulletDatabase Bullet;
    }
}
