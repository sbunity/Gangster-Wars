using DG.Tweening;
using SBabchuk.Runtime.Audio;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SBabchuk.Runtime.UI
{
    public abstract class CoinCounterView : MonoBehaviour
    {
        [SerializeField] private SoundConfig _addSound;
        [SerializeField, Min(0f)] private float _countUpDuration = 0.55f;

        private Text _txt;
        private CountPulse _countPulse;
        private IAudioService _audioService;
        private Tween _countTween;
        private int _displayed;

        protected virtual bool IgnoreTimeScale => false;

        [Inject]
        public void Construct(IAudioService audioService)
        {
            _audioService = audioService;
        }

        protected virtual void Awake()
        {
            _txt = GetComponentInChildren<Text>();
            _countPulse = GetOrAdd<CountPulse>();
            if (_txt != null)
                _countPulse.SetTarget(_txt.transform);
        }

        protected virtual void OnDisable()
        {
            _countTween?.Kill();
        }

        protected void AnimateTo(int value, float delay = 0f)
        {
            _countTween?.Kill();
            _countTween = DOTween.To(() => _displayed, SetDisplayed, value, _countUpDuration)
                .SetDelay(delay)
                .SetEase(Ease.OutQuad)
                .SetUpdate(IgnoreTimeScale)
                .OnComplete(() =>
                {
                    SetDisplayed(value);
                    PlayCollectFeedback();
                });
        }

        protected void SetImmediate(int value)
        {
            _countTween?.Kill();
            SetDisplayed(value);
        }

        protected virtual string Format(int value) => value.ToString();

        private void SetDisplayed(int value)
        {
            _displayed = value;
            if (_txt != null)
                _txt.text = Format(value);
        }

        private void PlayCollectFeedback()
        {
            _countPulse.Play(IgnoreTimeScale);
            _audioService?.Play(_addSound);
        }

        private T GetOrAdd<T>() where T : Component
            => TryGetComponent<T>(out var component) ? component : gameObject.AddComponent<T>();
    }
}
