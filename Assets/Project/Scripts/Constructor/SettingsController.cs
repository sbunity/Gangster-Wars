using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBabchuk
{
    public class SettingsController : MonoBehaviour
    {
        public static SettingsController Instance;

        [Header("Панел")]
        public GameObject panel;

        private void Awake()
        {
            Instance = this;
        }

        private void OnEnable()
        {
            ConstructorController.OnChangeVisibleSettings += ChangeVisible;
        }

        private void OnDisable()
        {
            ConstructorController.OnChangeVisibleSettings -= ChangeVisible;
        }

        private void ChangeVisible(bool _value)
        {
            panel.SetActive(_value);
        }

        public void Restart()
        {
            Time.timeScale = 1;
        }

        private void Start()
        {
            levelDatabase = PersistableSO.Instance.Level;
        }

        private LevelDatabase levelDatabase;

        /// <summary>
        /// Час створення хвилі
        /// </summary>
        private float timeCreate;

        /// <summary>
        /// ІД рівня
        /// </summary>
        public int levelID;

        /// <summary>
        /// ІД поточної хвилі
        /// </summary>
        private int waveID;

        /// <summary>
        /// Індекс юніта на хвилі
        /// </summary>
        private int indexEnemy;

        /// <summary>
        /// Створення нової хвилі
        /// </summary>
        public void CreateNewWave()
        {
            levelID = LevelController.c_levelID;

            waveID = levelDatabase.CreateWave(levelID, (int)(Time.time - timeCreate));

            timeCreate = Time.time;

            LevelController.Instance.ContinueLevel();
        }

        /// <summary>
        /// Добавляєм юніта в хвилю
        /// </summary>
        public void CreateNewEnemy(int _enemyID, int _count)
        {
            levelID = LevelController.c_levelID;

            indexEnemy = levelDatabase.CreateEnemyOnWave(levelID, waveID, _enemyID, _count, (int)(Time.time - timeCreate));

            LevelController.Instance.WaveHandler(waveID, indexEnemy);
        }
    }
}
