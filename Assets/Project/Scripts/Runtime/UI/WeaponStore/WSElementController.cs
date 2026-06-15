using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using SBabchuk.Runtime.Databases.PlayerPrefs;
using SBabchuk.Runtime.Databases.WeaponStore;
using SBabchuk.Runtime.Services;

namespace SBabchuk.Runtime.UI.WeaponStore
{
    public class WSElementController : StoreElementControllerBase
    {
        private const string InfiniteAmmoLabel = "∞";

        [SerializeField, FormerlySerializedAs("weapon")]
        private WeaponsName _weapon;

        [SerializeField, FormerlySerializedAs("ico")]
        private Image _icon;

        [SerializeField, FormerlySerializedAs("txt")]
        private Text _text;

        [SerializeField]
        private TMP_Text _ammoCountText;

        private SpriteSwap _panel;
        private LockElementController _lockElementController;
        private UnlockElementController _unlockElementController;
        private AmmunitionsController _ammunitionsController;
        private WeaponShortInfo _weaponShortInfo;
        private Weapon _weaponInfo;

        private void Start()
        {
            _panel = GetComponentInChildren<SpriteSwap>();
            _lockElementController = GetComponentInChildren<LockElementController>(true);
            _unlockElementController = GetComponentInChildren<UnlockElementController>(true);
            _ammunitionsController = GetComponentInChildren<AmmunitionsController>(true);
            var weaponStore = AssetProvider.WeaponStoreDatabase;
            _weaponInfo = weaponStore.GetWeapon((int)_weapon);
            _icon.sprite = _weaponInfo.Icon;
            _text.text = _weaponInfo.Name;
            RefreshState();
        }

        protected override void RefreshState()
        {
            _weaponShortInfo = ProgressService.GetWeaponShortInfo((int)_weapon);
            ChangeLock(_weaponShortInfo.IsBuy == mySwitch.On);
        }

        private void ChangeLock(bool value = false)
        {
            StoreElementView.Apply(
                _panel,
                _lockElementController.gameObject,
                _unlockElementController.gameObject,
                (int)_weapon,
                value,
                _lockElementController,
                _unlockElementController,
                _ammunitionsController);

            UpdateAmmoCount(value);
        }

        private void UpdateAmmoCount(bool isOwned)
        {
            if (_ammoCountText == null)
                return;

            var hasInfiniteAmmo = _weapon == WeaponsName.Weapon_1;
            var isVisible = isOwned || hasInfiniteAmmo;
            _ammoCountText.gameObject.SetActive(isVisible);

            if (!isVisible)
                return;

            _ammoCountText.text = hasInfiniteAmmo
                ? InfiniteAmmoLabel
                : (_weaponShortInfo?.AmmoCount ?? 0).ToString();
        }
    }
}
