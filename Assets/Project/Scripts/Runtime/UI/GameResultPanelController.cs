using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Audio;
using SBabchuk.Runtime.Services.Contracts;
using SBabchuk.Runtime.UI.Popups;
using SBabchuk.Runtime.Vibration;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

namespace SBabchuk.Runtime.UI
{
    public abstract class GameResultPanelController : MonoBehaviour
    {
        [SerializeField, FormerlySerializedAs("type")]
        private Panels _type;

        [SerializeField, FormerlySerializedAs("panel")]
        private GameObject _panel;

        [SerializeField, Min(0f)] private float _showDelay;

        [SerializeField] private SoundConfig _showSound;

        [SerializeField] private VibrationConfig _showVibration;

        [SerializeField] private LevelCoinsRewardView _coinsReward;

        [Header("Sorting")]
        [SerializeField] private Canvas _sortingCanvas;
        [SerializeField] private bool _overrideSorting = true;
        [SerializeField] private int _sortingOrder = 10000;

        private ISceneTransitionService _sceneTransitionService;
        private IAudioService _audioService;
        private IMusicService _musicService;
        private IVibrationService _vibrationService;
        private ILevelEndInterstitialService _interstitial;
        private SignalSubscriptions _signals;
        private Tween _showDelayTween;
        private readonly List<(Button Button, UnityAction Listener)> _choices = new();

        [Inject]
        public void Construct(ISceneTransitionService sceneTransitionService, IAudioService audioService, IMusicService musicService, IVibrationService vibrationService, ILevelEndInterstitialService interstitial, SignalBus signalBus)
        {
            _sceneTransitionService = sceneTransitionService;
            _audioService = audioService;
            _musicService = musicService;
            _vibrationService = vibrationService;
            _interstitial = interstitial;
            _signals = new SignalSubscriptions(signalBus)
                .Add<GameFinishedSignal>(OnGameFinished);
        }

        protected virtual void OnEnable() => _signals?.Enable();

        protected virtual void OnDisable() => _signals?.Disable();

        protected virtual void OnDestroy()
        {
            _showDelayTween?.Kill();

            foreach (var choice in _choices)
                choice.Button.onClick.RemoveListener(choice.Listener);
        }

        public void Show(Panels panelType)
        {
            if (_type != panelType)
                return;

            OnShow();
            ApplyTopMostSorting();
            var presentation = PlayShowAnimationAsync(destroyCancellationToken);
            Time.timeScale = 0f;
            _musicService?.Stop();
            _audioService?.Play(_showSound);
            _vibrationService?.Vibrate(_showVibration);
            _interstitial.ScheduleAfter(presentation);
        }

        public void Hide()
        {
            _showDelayTween?.Kill();
            _panel.SetActive(false);
        }

        protected virtual void OnShow() { }

        protected void AddChoice(Button button, Action action)
        {
            if (button == null)
                return;

            UnityAction listener = () => _interstitial.RunAfterAd(action);
            button.onClick.AddListener(listener);
            _choices.Add((button, listener));
        }

        protected void TransitionTo(Scene scene)
        {
            _sceneTransitionService?.TransitionToAsync(scene).Forget();
            Hide();
        }

        private void OnGameFinished(GameFinishedSignal signal) => ScheduleShow(signal.Panel);

        private void ScheduleShow(Panels panelType)
        {
            if (_type != panelType)
                return;

            _showDelayTween?.Kill();

            if (_showDelay <= 0f)
            {
                Show(panelType);
                return;
            }

            _showDelayTween = DOVirtual.DelayedCall(_showDelay, () => Show(panelType), false);
        }

        private void ApplyTopMostSorting()
        {
            transform.SetAsLastSibling();

            var canvas = ResolveSortingCanvas();
            if (canvas == null)
                return;

            canvas.overrideSorting = _overrideSorting;
            canvas.sortingOrder = _sortingOrder;

            var parentCanvas = transform.parent != null
                ? transform.parent.GetComponentInParent<Canvas>()
                : null;

            if (parentCanvas != null)
                canvas.sortingLayerID = parentCanvas.sortingLayerID;
        }

        private Canvas ResolveSortingCanvas()
        {
            if (_sortingCanvas != null)
                return _sortingCanvas;

            _sortingCanvas = GetComponent<Canvas>();
            if (_sortingCanvas == null)
                _sortingCanvas = gameObject.AddComponent<Canvas>();

            if (GetComponent<GraphicRaycaster>() == null)
                gameObject.AddComponent<GraphicRaycaster>();

            return _sortingCanvas;
        }

        private UniTask PlayShowAnimationAsync(CancellationToken cancellationToken)
            => UniTask.WhenAll(_panel.ShowPopupAsync(cancellationToken), PlayCoinsRewardAsync(cancellationToken));

        private UniTask PlayCoinsRewardAsync(CancellationToken cancellationToken)
            => _coinsReward != null ? _coinsReward.PlayAsync(cancellationToken) : UniTask.CompletedTask;
    }
}
