using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SBabchuk
{
    public class BttnSelectLvlController : MonoBehaviour
    {
        [Header("За який рівень відповідає")]
        public LevelsName level;

        [Header("Чи пройдений")]
        public mySwitch isCompleted;

        [Header("Назва рівня")]
        public Text tittle;

        [Header("Кaртинка заблокованого рiвня")]
        public GameObject lockImg;

        [Header("Кількість зірочок")]
        public Image stars;

        [Header("Кількість зірочок")]
        public List<Sprite> starsSprite;

        [Header("Залешності")]
        public List<BttnSelectLvlController> predecessors = new List<BttnSelectLvlController>();

        private LevelDatabase _database;
        private Level _properties;
        private IAssetProvider _assetProvider;
        private IPlayerProgressService _progressService;
        private ISceneLoaderService _sceneLoaderService;

        [Inject]
        private void Construct(
            IAssetProvider assetProvider,
            IPlayerProgressService progressService,
            ISceneLoaderService sceneLoaderService)
        {
            _assetProvider = assetProvider;
            _progressService = progressService;
            _sceneLoaderService = sceneLoaderService;
        }

        private IEnumerator Start()
        {
            _database = _assetProvider.LevelDatabase;
            Init(level);

            yield return null;

            RefreshLockState();
        }

        public void Init(LevelsName targetLevel)
        {
            _properties = _database.GetLevel((int)targetLevel);
            tittle.text = _properties.name;

            var levelShortInfo = _progressService.GetLevelShortInfo((int)targetLevel);
            isCompleted = levelShortInfo.isCompleted;
            stars.sprite = GetSpriteStar(levelShortInfo.stars);
        }

        public Sprite GetSpriteStar(int value)
        {
            return starsSprite[value];
        }

        public void RefreshLockState()
        {
            var isLocked = false;

            foreach (var predecessor in predecessors)
            {
                if (predecessor != null && predecessor.isCompleted != mySwitch.On)
                {
                    isLocked = true;
                    break;
                }
            }

            lockImg.SetActive(isLocked);
            stars.enabled = !lockImg.activeSelf;
        }

        public void StartLevel()
        {
            if (lockImg.activeSelf)
                return;

            _progressService.SetCurrentLevel((int)level);
            _sceneLoaderService.LoadAsync(Scene.GameScene).Forget();
        }
    }
}
