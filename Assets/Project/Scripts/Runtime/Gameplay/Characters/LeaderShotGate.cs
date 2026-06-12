namespace SBabchuk.Runtime.Gameplay.Characters
{
    public sealed class LeaderShotGate
    {
        private bool _isPressed;
        private bool _hasShotDuringPress;
        private bool _allowReleasedTapShot;

        public void Press()
        {
            if (_isPressed)
                return;

            _isPressed = true;
            _hasShotDuringPress = false;
            _allowReleasedTapShot = false;
        }

        public bool Release()
        {
            if (!_isPressed)
                return false;

            _isPressed = false;
            if (_hasShotDuringPress)
                return true;

            _allowReleasedTapShot = true;
            return false;
        }

        public bool Cancel()
        {
            var hadActiveShot = _isPressed || _hasShotDuringPress || _allowReleasedTapShot;

            _isPressed = false;
            _hasShotDuringPress = false;
            _allowReleasedTapShot = false;

            return hadActiveShot;
        }

        public bool TryConsumeShot(out bool shouldFinishAfterShot)
        {
            shouldFinishAfterShot = false;

            if (_isPressed)
            {
                _hasShotDuringPress = true;
                return true;
            }

            if (!_allowReleasedTapShot)
                return false;

            _allowReleasedTapShot = false;
            _hasShotDuringPress = true;
            shouldFinishAfterShot = true;
            return true;
        }
    }
}
