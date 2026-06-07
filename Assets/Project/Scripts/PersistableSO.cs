using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace SBabchuk
{
    public class PersistableSO : MonoBehaviour
    {
        [Header("База, що змынюэться ы ъъ потрыбно пыдгружати")]
        public static PersistableSO Instance;

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

        private void Awake()
        {
            Instance = this;

            DontDestroyOnLoad(this);
        }

        protected void OnEnable()
        {
            Load();
        }

        public void Load()
        {
            for (int i = 0; i < objectsToPersist.Count; i++)
            {
                LoadSO(objectsToPersist[i]);
            }
        }

        public void LoadSO(ScriptableObject _objectsToPersist)
        {
            if (File.Exists(Application.persistentDataPath + string.Format("/{0}_{1}.pso", "Main", _objectsToPersist.name)))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + string.Format("/{0}_{1}.pso", "Main", _objectsToPersist.name), FileMode.Open);
                JsonUtility.FromJsonOverwrite((string)bf.Deserialize(file), _objectsToPersist);
                file.Close();

            }
            else
            {
                //Do Nothing
            }
        }

        public void Save()
        {
            for (int i = 0; i < objectsToPersist.Count; i++)
            {
                SaveSO(objectsToPersist[i]);
            }
        }

        public void SaveSO(ScriptableObject _objectsToPersist)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + string.Format("/{0}_{1}.pso", "Main", _objectsToPersist.name));
            var json = JsonUtility.ToJson(_objectsToPersist);
            bf.Serialize(file, json);
            file.Close();
        }
    }
}
