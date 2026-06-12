using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using SBabchuk.Runtime.Databases.DefenseStore;
using SBabchuk.Runtime.Databases.PlayerPrefs;
using SBabchuk.Runtime.Services;

namespace SBabchuk.Runtime.UI.WeaponStore
{
    public class DSElementController : StoreElementControllerBase
    {
        [SerializeField, FormerlySerializedAs("defence")]
        private DefencesName _defence;

        [SerializeField, FormerlySerializedAs("ico")]
        private Image _icon;

        [SerializeField, FormerlySerializedAs("txt")]
        private Text _text;

        private SpriteSwap _panel;
        private UnlockDElementController _unlockDElementController;
        private LockDElementController _lockDElementController;
        private AmmunitionsController _ammunitionsController;
        private DefenceShortInfo _defenceShortInfo;
        private Defense _defenceInfo;

        private void Start()
        {
            _panel = GetComponentInChildren<SpriteSwap>();
            _unlockDElementController = GetComponentInChildren<UnlockDElementController>(true);
            _lockDElementController = GetComponentInChildren<LockDElementController>(true);
            var defenceStore = AssetProvider.DefenseStoreDatabase;
            _defenceInfo = defenceStore.GetDefense((int)_defence);
            _ammunitionsController = GetComponentInChildren<AmmunitionsController>(true);
            _icon.sprite = _defenceInfo.Icon;
            _text.text = _defenceInfo.Name;
            RefreshState();
        }

        protected override void RefreshState()
        {
            _defenceShortInfo = ProgressService.GetDefenceShortInfo((int)_defence);
            ChangeLock(_defenceShortInfo.IsBuy == mySwitch.On);
        }

        private void ChangeLock(bool _value = false)
        {
            ApplyLockState(_panel, _lockDElementController.gameObject, _unlockDElementController.gameObject, _value);
            if (_lockDElementController.gameObject.activeSelf)
                _lockDElementController.Initialisation((int)_defence);
                
            if (_unlockDElementController.gameObject.activeSelf)
            {
                _unlockDElementController.Initialisation((int)_defence);
                _ammunitionsController.Initialisation((int)_defence);
            }
        }
    }
}
