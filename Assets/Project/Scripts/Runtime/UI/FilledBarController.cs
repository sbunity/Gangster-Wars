using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;

namespace SBabchuk
{
    public class FilledBarController : MonoBehaviour
    {
        [SerializeField, FormerlySerializedAs("slider")]
        private Slider _slider;

        [SerializeField, FormerlySerializedAs("speed"), Range(0.1f, 3)]
        private float _speed;

        private float _currentvalue;
        private Tween _twn;

        public void UpdateFlled(float value)
        {
            _twn?.Kill();
            _currentvalue += value;
            _twn = _slider.DOValue(_currentvalue, _speed).SetUpdate(true);
        }
    }
}
