using Cysharp.Threading.Tasks;
using DG.Tweening;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace SBabchuk.Runtime.UI
{
    public abstract class GameResultPanelController : MonoBehaviour
    {
        [SerializeField, FormerlySerializedAs("type")]
        private Panels _type;

        [SerializeField, FormerlySerializedAs("panel")]
        private GameObject _panel;

        [SerializeField] private CanvasGroup _canvasGroup;

        [SerializeField, Min(0f)] private float _fadeDuration = 0.4f;

        private ISceneTransitionService _sceneTransitionService;
        private SignalSubscriptions _signals;
        private Tween _fadeTween;

        [Inject]
        public void Construct(ISceneTransitionService sceneTransitionService, SignalBus signalBus)
        {
            _sceneTransitionService = sceneTransitionService;
            _signals = new SignalSubscriptions(signalBus)
                .Add<GameFinishedSignal>(OnGameFinished);
        }

        protected virtual void OnEnable() => _signals?.Enable();

        protected virtual void OnDisable() => _signals?.Disable();

        protected virtual void OnDestroy() => _fadeTween?.Kill();

        public void Show(Panels panelType)
        {
            if (_type != panelType)
                return;

            OnShow();
            _panel.SetActive(true);
            Time.timeScale = 0f;
            PlayFadeIn();
        }

        public void Hide()
        {
            _fadeTween?.Kill();
            _panel.SetActive(false);
        }

        /// <summary>Hook for concrete panels to refresh their content right before the panel appears.</summary>
        protected virtual void OnShow() { }

        /// <summary>Resumes time and transitions to the target scene, then hides the panel.</summary>
        protected void TransitionTo(Scene scene)
        {
            Time.timeScale = 1f;
            _sceneTransitionService?.TransitionToAsync(scene).Forget();
            Hide();
        }

        private void OnGameFinished(GameFinishedSignal signal) => Show(signal.Panel);

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
