using UnityEngine;

namespace SBabchuk.Runtime.Tutorial
{
    public sealed class TutorialTimeScale
    {
        private float _previousScale = 1f;
        private float _appliedScale;
        private bool _isApplied;

        public void SlowDown(float scale)
        {
            if (_isApplied)
                return;

            _previousScale = Time.timeScale;
            _appliedScale = scale;
            _isApplied = true;
            Time.timeScale = scale;
        }

        public void Restore()
        {
            if (!_isApplied)
                return;

            _isApplied = false;
            if (Mathf.Approximately(Time.timeScale, _appliedScale))
                Time.timeScale = _previousScale;
        }
    }
}
