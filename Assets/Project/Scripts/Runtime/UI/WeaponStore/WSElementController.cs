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
        [SerializeField, FormerlySerializedAs("weapon")]
        private WeaponsName _weapon;

        [SerializeField, FormerlySerializedAs("ico")]
        private Image _icon;

        [SerializeField, FormerlySerializedAs("txt")]
        private Text _text;

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
            ApplyLockState(_panel, _lockElementController.gameObject, _unlockElementController.gameObject, value);
            
            if (_lockElementController.gameObject.activeSelf)
                _lockElementController.Initialisation((int)_weapon);

            if (_unlockElementController.gameObject.activeSelf)
            {
                _unlockElementController.Initialisation((int)_weapon);
                _ammunitionsController.Initialisation((int)_weapon);
            }
        }
    }
}
