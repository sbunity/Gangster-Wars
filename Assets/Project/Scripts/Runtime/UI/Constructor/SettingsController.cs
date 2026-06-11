using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using Zenject;

namespace SBabchuk
{
    public class SettingsController : MonoBehaviour
    {
        [Header("Панел")]
        public GameObject panel;

        private LevelDatabase _levelDatabase;
        private IPlayerProgressService _progressService;
        private LevelController _levelController;

        [Header("Час створення хвилі")]
        [SerializeField] private float _timeCreate;

        [Header("ІД рівня")]
        public int levelID;

        [Header("ІД поточної хвилі")]
        [SerializeField] private int _waveID;

        [Header("Індекс юніта на хвилі")]
        [SerializeField] private int _indexEnemy;

        [Inject]
        private void Construct(
            IAssetProvider assetProvider,
            IPlayerProgressService progressService,
            LevelController levelController)
        {
            _levelDatabase = assetProvider.LevelDatabase;
            _progressService = progressService;
            _levelController = levelController;
        }

        public void ChangeVisible(bool value)
        {
            panel.SetActive(value);
        }

        public void Restart()
        {
            Time.timeScale = 1;
        }

        public void CreateNewWave()
        {
            levelID = _progressService.CurrentLevelId;

            _waveID = _levelDatabase.CreateWave(levelID, (int)(Time.time - _timeCreate));
            _timeCreate = Time.time;

            _levelController.ContinueLevel();
        }

        public void CreateNewEnemy(int enemyID, int count)
        {
            levelID = _progressService.CurrentLevelId;

            _indexEnemy = _levelDatabase.CreateEnemyOnWave(levelID, _waveID, enemyID, count, (int)(Time.time - _timeCreate));
            _levelController.WaveHandler(_waveID, _indexEnemy);
        }
    }
}
