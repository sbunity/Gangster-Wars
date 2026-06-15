using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UnityEngine.Serialization;

namespace SBabchuk.Runtime.UI
{
    public class WinPanelController : MonoBehaviour
    {
        [SerializeField, FormerlySerializedAs("type")]
        private Panels _type;

        [SerializeField, FormerlySerializedAs("panel")]
        private GameObject _panel;

        [SerializeField] private CanvasGroup _canvasGroup;

        [SerializeField, Min(0f)] private float _fadeDuration = 0.4f;

        [SerializeField] private Button _continueButton;

        [Header("Stars (Win panel only)")]
        [SerializeField] private Image _starsImage;

        [SerializeField] private List<Sprite> _starsSprites;

        private ISceneTransitionService _sceneTransitionService;
        private IPlayerProgressService _progressService;
        private SignalSubscriptions _signals;
        private Tween _fadeTween;

        [Inject]
        public void Construct(
            ISceneTransitionService sceneTransitionService,
            IPlayerProgressService progressService,
            SignalBus signalBus)
        {
            _sceneTransitionService = sceneTransitionService;
            _progressService = progressService;
            _signals = new SignalSubscriptions(signalBus)
                .Add<GameFinishedSignal>(OnGameFinished);
        }

        private void Awake()
        {
            if (_continueButton != null)
                _continueButton.onClick.AddListener(SwitchScene);
        }

        private void OnEnable() => _signals?.Enable();

        private void OnDisable() => _signals?.Disable();

        private void OnDestroy()
        {
            _fadeTween?.Kill();
            if (_continueButton != null)
                _continueButton.onClick.RemoveListener(SwitchScene);
        }

        public void Show(Panels panelType)
        {
            if (_type != panelType)
                return;

            UpdateStars();
            _panel.SetActive(true);
            Time.timeScale = 0f;
            PlayFadeIn();
        }

        public void Hide()
        {
            _fadeTween?.Kill();
            _panel.SetActive(false);
        }

        public void SwitchScene()
        {
            Time.timeScale = 1f;
            _sceneTransitionService?.TransitionToAsync(Scene.MainScene).Forget();
            Hide();
        }

        private void OnGameFinished(GameFinishedSignal signal)
        {
            Show(signal.Panel);
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

        private void PlayFadeIn()
        {
            if (_canvasGroup == null)
                return;

            _fadeTween?.Kill();
            _canvasGroup.alpha = 0f;
            _fadeTween = DOTween
                .To(() => _canvasGroup.alpha, value => _canvasGroup.alpha = value, 1f, _fadeDuration)
                .SetUpdate(true);
        }
    }
}
