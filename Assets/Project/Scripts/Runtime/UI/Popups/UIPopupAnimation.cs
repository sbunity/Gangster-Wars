using DG.Tweening;
using UnityEngine;

namespace SBabchuk.Runtime.UI.Popups
{
    [RequireComponent(typeof(CanvasGroup))]
    public sealed class UIPopupAnimation : MonoBehaviour
    {
        [SerializeField] private UIPopupStyle _style;
        [SerializeField] private RectTransform _window;

        private CanvasGroup _canvasGroup;
        private Sequence _sequence;
        private Vector3 _windowRestScale;
        private bool _hasWindowRestScale;
        private float _scale = 1f;
        private bool _isHiding;

        public bool IsShown => gameObject.activeSelf && !_isHiding;

        private CanvasGroup CanvasGroup => _canvasGroup != null ? _canvasGroup : _canvasGroup = GetComponent<CanvasGroup>();

        public void Show()
        {
            if (IsShown)
                return;

            Kill();
            _isHiding = false;
            SetInteractive(true);

            if (!gameObject.activeSelf)
            {
                SetScale(_style.ShowFromScale);
                CanvasGroup.alpha = 0f;
                gameObject.SetActive(true);
            }

            _sequence = DOTween.Sequence()
                .SetUpdate(true)
                .Join(CanvasGroup.DOFade(1f, _style.FadeInDuration))
                .Join(DOVirtual.Float(_scale, 1f, _style.ShowDuration, SetScale).SetEase(_style.ShowEase, _style.ShowOvershoot))
                .OnComplete(() => _sequence = null);
        }

        public void Hide()
        {
            if (!IsShown)
                return;

            Kill();
            _isHiding = true;
            SetInteractive(false);

            _sequence = DOTween.Sequence()
                .SetUpdate(true)
                .Join(CanvasGroup.DOFade(0f, _style.HideDuration))
                .Join(DOVirtual.Float(_scale, _style.HideToScale, _style.HideDuration, SetScale).SetEase(_style.HideEase))
                .OnComplete(CompleteHide);
        }

        private void OnDisable()
        {
            Kill();
            _isHiding = false;
            SetScale(1f);
            CanvasGroup.alpha = 1f;
            SetInteractive(true);
        }

        private void OnDestroy() => Kill();

        private void CompleteHide()
        {
            _sequence = null;
            gameObject.SetActive(false);
        }

        private void Kill()
        {
            _sequence?.Kill();
            _sequence = null;
        }

        private void SetScale(float scale)
        {
            _scale = scale;

            if (_window == null)
                return;

            if (!_hasWindowRestScale)
            {
                _windowRestScale = _window.localScale;
                _hasWindowRestScale = true;
            }

            _window.localScale = new Vector3(_windowRestScale.x * scale, _windowRestScale.y * scale, _windowRestScale.z);
        }

        private void SetInteractive(bool value)
        {
            CanvasGroup.interactable = value;
            CanvasGroup.blocksRaycasts = value;
        }
    }
}
