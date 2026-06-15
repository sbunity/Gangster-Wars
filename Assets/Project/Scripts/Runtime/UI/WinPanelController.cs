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

        [Header("Stars (Win panel only)")]
        [SerializeField] private Image _starsImage;

        [SerializeField] private List<Sprite> _starsSprites;

        private IPlayerProgressService _progressService;

        [Inject]
        public void Construct(IPlayerProgressService progressService)
        {
            _progressService = progressService;
        }

        private void Awake()
        {
            if (_continueButton != null)
                _continueButton.onClick.AddListener(ReturnToMainMenu);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (_continueButton != null)
                _continueButton.onClick.RemoveListener(ReturnToMainMenu);
        }

        protected override void OnShow() => UpdateStars();

        public void ReturnToMainMenu() => TransitionTo(Scene.MainScene);

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
