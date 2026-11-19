using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SBabchuk.Runtime.UI.WeaponStore
{
    public sealed class RewardedCoinsElementView : MonoBehaviour
    {
        [SerializeField] private Button _watchButton;
        [SerializeField] private Text _amountText;

        private IRewardedCoinsService _rewardedCoins;
        private int _shownAmount = -1;

        [Inject]
        public void Construct(IRewardedCoinsService rewardedCoins)
        {
            _rewardedCoins = rewardedCoins;
        }

        private void Awake()
        {
            _watchButton.onClick.AddListener(Watch);
        }

        private void OnDestroy()
        {
            _watchButton.onClick.RemoveListener(Watch);
        }

        private void OnEnable() => Refresh();

        private void Update() => Refresh();

        private void Refresh()
        {
            if (_rewardedCoins == null)
                return;

            _watchButton.interactable = _rewardedCoins.CanWatch;
            ShowAmount(_rewardedCoins.Amount);
        }

        private void ShowAmount(int amount)
        {
            if (amount == _shownAmount)
                return;

            _shownAmount = amount;
            _amountText.text = amount.ToString();
        }

        private void Watch() => _rewardedCoins.Watch();
    }
}
