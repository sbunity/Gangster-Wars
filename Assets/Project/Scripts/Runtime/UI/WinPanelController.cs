using System.Collections.Generic;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SBabchuk.Runtime.UI
{
    public class WinPanelController : GameResultPanelController
    {
        [SerializeField] private Button _continueButton;

        [SerializeField] private Button _nextLevelButton;

        [Header("Stars (Win panel only)")]
        [SerializeField] private Image _starsImage;

        [SerializeField] private List<Sprite> _starsSprites;

        private IPlayerProgressService _progressService;
        private ITutorialProgressService _tutorialProgress;
        private IAssetProvider _assetProvider;
        private int _nextLevelId = -1;

        [Inject]
        public void Construct(IPlayerProgressService progressService, ITutorialProgressService tutorialProgress, IAssetProvider assetProvider)
        {
            _progressService = progressService;
            _tutorialProgress = tutorialProgress;
            _assetProvider = assetProvider;
        }

        private void Awake()
        {
            if (_continueButton != null)
                _continueButton.onClick.AddListener(ReturnToMainMenu);

            if (_nextLevelButton != null)
                _nextLevelButton.onClick.AddListener(StartNextLevel);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (_continueButton != null)
                _continueButton.onClick.RemoveListener(ReturnToMainMenu);

            if (_nextLevelButton != null)
                _nextLevelButton.onClick.RemoveListener(StartNextLevel);
        }

        protected override void OnShow()
        {
            UpdateStars();
            UpdateNextLevelButton();
        }

        public void ReturnToMainMenu() => TransitionTo(Scene.MainScene);

        public void StartNextLevel()
        {
            if (_nextLevelId < 0)
                return;

            _progressService.SetCurrentLevel(_nextLevelId);
            TransitionTo(Scene.GameScene);
        }

        private void UpdateNextLevelButton()
        {
            if (_nextLevelButton == null)
                return;

            _nextLevelButton.gameObject.SetActive(TryGetNextLevel(out _nextLevelId));
        }

        private bool TryGetNextLevel(out int levelId)
        {
            levelId = -1;
            return _tutorialProgress.IsMenuTutorialCompleted
                && _assetProvider.LevelDatabase.TryGetNextLevelId(_progressService.CurrentLevelId, out levelId);
        }

        private void UpdateStars()
        {
            if (_starsImage == null || _starsSprites == null || _starsSprites.Count == 0)
                return;

            var levelInfo = _progressService?.GetLevelShortInfo(_progressService.CurrentLevelId);
            var stars = levelInfo != null ? levelInfo.Stars : 0;
            stars = Mathf.Clamp(stars, 0, _starsSprites.Count - 1);
            _starsImage.sprite = _starsSprites[stars];
        }
    }
}
