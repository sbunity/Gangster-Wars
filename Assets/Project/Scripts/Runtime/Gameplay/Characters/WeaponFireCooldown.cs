namespace SBabchuk.Runtime.Gameplay.Characters
{
    public sealed class WeaponFireCooldown
    {
        private float _interval;
        private float _lastShotTime = float.NegativeInfinity;

        public void SetFireRate(float shotsPerSecond)
        {
            _interval = shotsPerSecond > 0f ? 1f / shotsPerSecond : 0f;
        }

        public bool IsReady(float time)
            => time >= ReadyTime;

        public bool TryConsume(float time, float frameTime)
        {
            if (!IsReady(time))
                return false;

            _lastShotTime = time - ReadyTime < frameTime ? ReadyTime : time;
            return true;
        }

        private float ReadyTime => _lastShotTime + _interval;
    }
}
