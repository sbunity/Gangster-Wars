using Cysharp.Threading.Tasks;
using DG.Tweening;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;

namespace SBabchuk.Runtime.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public sealed class LoadingScreenView : MonoBehaviour, ILoadingScreen
    {
        [SerializeField]
        private CanvasGroup _canvasGroup;

        [SerializeField, Min(0f)]
        private float _fadeDuration = 0.35f;

        private Tween _fadeTween;

        private void Reset() => _canvasGroup = GetComponent<CanvasGroup>();

        private void Awake()
        {
            if (_canvasGroup == null)
                _canvasGroup = GetComponent<CanvasGroup>();

            _canvasGroup.alpha = 0f;
            SetBlocking(false);
            gameObject.SetActive(false);
        }

        public UniTask ShowAsync()
        {
            gameObject.SetActive(true);
            SetBlocking(true);
            return FadeAsync(1f);
        }

        public async UniTask HideAsync()
        {
            await FadeAsync(0f);
            SetBlocking(false);
            gameObject.SetActive(false);
        }

        private async UniTask FadeAsync(float targetAlpha)
        {
            _fadeTween?.Kill();

            var completion = new UniTaskCompletionSource();
            _fadeTween = DOTween
                .To(() => _canvasGroup.alpha, value => _canvasGroup.alpha = value, targetAlpha, _fadeDuration)
                .SetUpdate(true)
                .OnComplete(() => completion.TrySetResult());

            await completion.Task;
        }

        private void SetBlocking(bool value)
        {
            _canvasGroup.blocksRaycasts = value;
            _canvasGroup.interactable = value;
        }

        private void OnDestroy() => _fadeTween?.Kill();
    }
}
