using System.Collections;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;

namespace SBabchuk.Runtime.Gameplay.Cameras
{
    [DisallowMultipleComponent]
    public sealed class CameraShakeController : MonoBehaviour, ICameraShakeService
    {
        [SerializeField] private Transform _target;
        [SerializeField, Min(0.01f)] private float _defaultDuration = 0.16f;
        [SerializeField, Min(1f)] private float _frequency = 38f;

        private Coroutine _shakeRoutine;
        private Vector3 _originLocalPosition;

        private void Awake()
        {
            if (_target == null)
                _target = transform;

            CaptureOrigin();
        }

        private void OnDisable()
        {
            StopShake();
            RestoreOrigin();
        }

        public void Shake(float strength)
        {
            Shake(strength, _defaultDuration);
        }

        public void Shake(float strength, float duration)
        {
            if (!isActiveAndEnabled || _target == null)
                return;

            strength = Mathf.Max(0f, strength);
            duration = Mathf.Max(0f, duration);

            if (strength <= 0f || duration <= 0f)
                return;

            StopShake();
            CaptureOrigin();
            _shakeRoutine = StartCoroutine(ShakeRoutine(strength, duration));
        }

        private IEnumerator ShakeRoutine(float strength, float duration)
        {
            var elapsed = 0f;
            var seed = Random.value * 100f;

            while (elapsed < duration)
            {
                var progress = elapsed / duration;
                var amplitude = strength * (1f - progress);
                var sample = (Time.unscaledTime + seed) * _frequency;
                var offset = new Vector3(
                    (Mathf.PerlinNoise(sample, seed) - 0.5f) * 2f,
                    (Mathf.PerlinNoise(seed, sample) - 0.5f) * 2f,
                    0f) * amplitude;

                _target.localPosition = _originLocalPosition + offset;
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            RestoreOrigin();
            _shakeRoutine = null;
        }

        private void StopShake()
        {
            if (_shakeRoutine == null)
                return;

            StopCoroutine(_shakeRoutine);
            _shakeRoutine = null;
            RestoreOrigin();
        }

        private void CaptureOrigin()
        {
            if (_target != null)
                _originLocalPosition = _target.localPosition;
        }

        private void RestoreOrigin()
        {
            if (_target != null)
                _target.localPosition = _originLocalPosition;
        }
    }
}
