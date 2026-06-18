using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SBabchuk.Runtime.UI.WeaponStore.Info
{
    [RequireComponent(typeof(Button))]
    public sealed class StoreInfoButtonController : MonoBehaviour
    {
        [SerializeField] private StoreInfoItemType _itemType;
        [SerializeField] private int _itemId;
        [SerializeField] private StoreInfoPopupView _popup;

        private Button _button;
        private IStoreItemStatsService _statsService;

        [Inject]
        public void Construct(IStoreItemStatsService statsService)
        {
            _statsService = statsService;
        }

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OpenPopup);
        }

        private void OnDestroy()
        {
            if (_button)
                _button.onClick.RemoveListener(OpenPopup);
        }

        public void Configure(StoreInfoItemType itemType, int itemId, StoreInfoPopupView popup)
        {
            _itemType = itemType;
            _itemId = itemId;
            _popup = popup;
        }

        private void OpenPopup()
        {
            if (_popup == null || _statsService == null)
                return;

            var target = transform.parent as RectTransform ?? transform as RectTransform;
            _popup.Show(_statsService.GetStats(_itemType, _itemId), target);
        }
    }
}
