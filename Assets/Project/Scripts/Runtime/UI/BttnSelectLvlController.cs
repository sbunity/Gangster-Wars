using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UnityEngine.Serialization;
using SBabchuk.Runtime.Databases.Levels;

namespace SBabchuk.Runtime.UI
{
    public class BttnSelectLvlController : MonoBehaviour
    {
        [SerializeField]
        private int _levelId = -1;

        [SerializeField, FormerlySerializedAs("level")]
        private LevelsName _level;

        [SerializeField, FormerlySerializedAs("isCompleted")]
        private mySwitch _isCompleted;
        public mySwitch IsCompleted { get => _isCompleted; set => _isCompleted = value; }

        [SerializeField, FormerlySerializedAs("tittle")]
        private Text _title;

        [SerializeField, FormerlySerializedAs("lockImg")]
        private GameObject _lockImg;

        [SerializeField, FormerlySerializedAs("stars")]
        private Image _stars;

        [SerializeField, FormerlySerializedAs("starsSprite")]
        private List<Sprite> _starsSprite;

        [SerializeField, FormerlySerializedAs("predecessors")]
        private List<BttnSelectLvlController> _predecessors = new List<BttnSelectLvlController>();

        [SerializeField]
        private UIPulseAnimation _nextLevelPulse;

        private LevelDatabase _database;
        private Level _properties;
        private IAssetProvider _assetProvider;
        private IPlayerProgressService _progressService;
        private ISceneTransitionService _sceneTransitionService;
        private ISceneIntro _sceneIntro;

        public int LevelId => _levelId >= 0 ? _levelId : (int)_level;

        [Inject]
        public void Construct(IAssetProvider assetProvider, IPlayerProgressService progressService, ISceneTransitionService sceneTransitionService, ISceneIntro sceneIntro)
        {
            _assetProvider = assetProvider;
            _progressService = progressService;
            _sceneTransitionService = sceneTransitionService;
            _sceneIntro = sceneIntro;
        }

        private IEnumerator Start()
        {
            _database = _assetProvider.LevelDatabase;
            Init(LevelId);

            yield return null;

            RefreshLockState();
            PulseIfNextLevelAsync().Forget();
        }

        public void Init(LevelsName targetLevel)
            => Init((int)targetLevel);

        public void Init(int levelId)
        {
            _levelId = levelId;
            _properties = _database.GetLevel(levelId);
            if (_properties == null)
                return;

            _title.text = _properties.Name;
            var levelShortInfo = _progressService.GetLevelShortInfo(levelId);
            if (levelShortInfo == null)
                return;

            _isCompleted = levelShortInfo.IsCompleted;
            _stars.sprite = GetSpriteStar(levelShortInfo.Stars);
        }

        public void SetPredecessors(List<BttnSelectLvlController> predecessors)
        {
            _predecessors = predecessors ?? new List<BttnSelectLvlController>();
        }

        public Sprite GetSpriteStar(int value)
        {
            return _starsSprite[value];
        }

        public void RefreshLockState()
        {
            var isLocked = false;
            foreach (var predecessor in _predecessors)
            {
                if (predecessor != null && predecessor.IsCompleted != mySwitch.On)
                {
                    isLocked = true;
                    break;
                }
            }

            _lockImg.SetActive(isLocked);
            _stars.enabled = !_lockImg.activeSelf;
        }

        private async UniTaskVoid PulseIfNextLevelAsync()
        {
            if (_nextLevelPulse == null || _lockImg.activeSelf || _isCompleted == mySwitch.On)
                return;

            try
            {
                await _sceneIntro.WaitForCompletionAsync(destroyCancellationToken);
                _nextLevelPulse.Play();
            }
            catch (OperationCanceledException)
            {
            }
        }

        public void StartLevel()
        {
            if (_lockImg.activeSelf)
                return;
                
            _progressService.SetCurrentLevel(LevelId);
            _sceneTransitionService.TransitionToAsync(Scene.GameScene).Forget();
        }
    }
}
