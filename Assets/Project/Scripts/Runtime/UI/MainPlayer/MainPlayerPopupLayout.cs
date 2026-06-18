using UnityEngine;

namespace SBabchuk.Runtime.UI.MainPlayer
{
    public sealed class MainPlayerPopupLayout : MonoBehaviour
    {
        private const float MinimumHeight = 720f;
        private const float BottomPadding = 38f;
        private const float UnlockedStatsY = -155f;
        private const float UnlockedBuyUpgradeY = -280f;
        private const float LockedStatsY = -130f;
        private const float LockedBuyElementY = -255f;
        private const float StatsHeight = 98f;

        [SerializeField] private RectTransform _pistolRoot;
        [SerializeField] private RectTransform _panel;
        [SerializeField] private RectTransform _unlockRoot;
        [SerializeField] private RectTransform _lockRoot;
        [SerializeField] private RectTransform _unlockedStats;
        [SerializeField] private RectTransform _lockedStats;
        [SerializeField] private RectTransform _buyUpgrade;
        [SerializeField] private RectTransform _lockedBuyElement;

        private RectTransform _root;
        private bool _lastUnlockedState;
        private bool _hasLastState;

        private void Awake()
        {
            ResolveReferences();
        }

        private void OnEnable()
        {
            ApplyLayout();
        }

        private void LateUpdate()
        {
            if (!_unlockRoot)
                return;

            var unlockedState = _unlockRoot.gameObject.activeSelf;
            if (_hasLastState && _lastUnlockedState == unlockedState)
                return;

            ApplyLayout();
        }

        public void ApplyLayout()
        {
            ResolveReferences();
            ApplyElementPositions();
            ApplyHeight();

            if (_unlockRoot)
            {
                _lastUnlockedState = _unlockRoot.gameObject.activeSelf;
                _hasLastState = true;
            }
        }

        private void ResolveReferences()
        {
            _root = _root ? _root : transform as RectTransform;
            _pistolRoot = _pistolRoot ? _pistolRoot : transform.Find("Pistol") as RectTransform;
            _panel = _panel ? _panel : transform.Find("Pistol/Panel") as RectTransform;
            _unlockRoot = _unlockRoot ? _unlockRoot : transform.Find("Pistol/UnLockElements") as RectTransform;
            _lockRoot = _lockRoot ? _lockRoot : transform.Find("Pistol/LockElements") as RectTransform;
            _unlockedStats = _unlockedStats ? _unlockedStats : transform.Find("Pistol/UnLockElements/StatsUnlocked") as RectTransform;
            _lockedStats = _lockedStats ? _lockedStats : transform.Find("Pistol/LockElements/StatsLocked") as RectTransform;
            _buyUpgrade = _buyUpgrade ? _buyUpgrade : transform.Find("Pistol/UnLockElements/BuyUpgrade") as RectTransform;
            _lockedBuyElement = _lockedBuyElement ? _lockedBuyElement : transform.Find("Pistol/LockElements/BuyElement") as RectTransform;
        }

        private void ApplyElementPositions()
        {
            if (_unlockedStats)
            {
                _unlockedStats.anchoredPosition = new Vector2(50f, UnlockedStatsY);
                _unlockedStats.sizeDelta = new Vector2(_unlockedStats.sizeDelta.x, StatsHeight);
            }

            if (_buyUpgrade)
                _buyUpgrade.anchoredPosition = new Vector2(_buyUpgrade.anchoredPosition.x, UnlockedBuyUpgradeY);

            if (_lockedStats)
            {
                _lockedStats.anchoredPosition = new Vector2(36f, LockedStatsY);
                _lockedStats.sizeDelta = new Vector2(_lockedStats.sizeDelta.x, StatsHeight);
            }

            if (_lockedBuyElement)
                _lockedBuyElement.anchoredPosition = new Vector2(_lockedBuyElement.anchoredPosition.x, LockedBuyElementY);
        }

        private void ApplyHeight()
        {
            if (!_root)
                return;

            var lowestY = 0f;
            if (!_lockRoot || !_lockRoot.gameObject.activeSelf)
            {
                CaptureLowestY(_unlockedStats, ref lowestY);
                CaptureLowestY(_buyUpgrade, ref lowestY);
            }

            if (_lockRoot && _lockRoot.gameObject.activeSelf)
            {
                CaptureLowestY(_lockedStats, ref lowestY);
                CaptureLowestY(_lockedBuyElement, ref lowestY);
            }

            var desiredHeight = Mathf.Max(MinimumHeight, Mathf.Abs(lowestY) * 2f + BottomPadding);
            SetHeight(_root, desiredHeight);
            SetHeight(_pistolRoot, desiredHeight);
            SetHeight(_panel, desiredHeight);
        }

        private void CaptureLowestY(RectTransform rectTransform, ref float lowestY)
        {
            if (!rectTransform)
                return;

            var bottom = rectTransform.anchoredPosition.y - rectTransform.rect.height * (1f - rectTransform.pivot.y);
            lowestY = Mathf.Min(lowestY, bottom);
        }

        private void SetHeight(RectTransform rectTransform, float height)
        {
            if (!rectTransform)
                return;

            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, height);
        }
    }
}
