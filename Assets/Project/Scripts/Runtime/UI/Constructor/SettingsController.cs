using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using Zenject;
using UnityEngine.Serialization;

namespace SBabchuk
{
    public class SettingsController : MonoBehaviour
    {
        [SerializeField, FormerlySerializedAs("panel")]
        private GameObject _panel;

        [SerializeField, FormerlySerializedAs("levelID")]
        private int _levelId;

        [SerializeField] private float _timeCreate;
        [SerializeField] private int _waveID;
        [SerializeField] private int _indexEnemy;

        private LevelDatabase _levelDatabase;
        private IPlayerProgressService _progressService;
        private LevelController _levelController;
        
        [Inject]
        public void Construct(IAssetProvider assetProvider, IPlayerProgressService progressService, LevelController levelController)
        {
            _levelDatabase = assetProvider.LevelDatabase;
            _progressService = progressService;
            _levelController = levelController;
        }

        public void ChangeVisible(bool value)
        {
            _panel.SetActive(value);
        }

        public void Restart()
        {
            Time.timeScale = 1;
        }

        public void CreateNewWave()
        {
            _levelId = _progressService.CurrentLevelId;
            _waveID = _levelDatabase.CreateWave(_levelId, (int)(Time.time - _timeCreate));
            _timeCreate = Time.time;
            _levelController.ContinueLevel();
        }

        public void CreateNewEnemy(int enemyID, int count)
        {
            _levelId = _progressService.CurrentLevelId;
            _indexEnemy = _levelDatabase.CreateEnemyOnWave(_levelId, _waveID, enemyID, count, (int)(Time.time - _timeCreate));
            _levelController.WaveHandler(_waveID, _indexEnemy);
        }
    }
}
