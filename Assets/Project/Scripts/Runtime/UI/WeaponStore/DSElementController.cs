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

        protected override void CacheView()
        {
            _panel = GetComponentInChildren<SpriteSwap>();
            _unlockDElementController = GetComponentInChildren<UnlockDElementController>(true);
            _lockDElementController = GetComponentInChildren<LockDElementController>(true);
            _ammunitionsController = GetComponentInChildren<AmmunitionsController>(true);
        }

        protected override void BindDefinition()
        {
            _defenceInfo = AssetProvider.DefenseStoreDatabase.GetDefense((int)_defence);
            if (_defenceInfo == null)
                return;

            _icon.sprite = _defenceInfo.Icon;
            _text.text = _defenceInfo.Name;
        }

        protected override void RefreshState()
        {
            _defenceShortInfo = ProgressService.GetDefenceShortInfo((int)_defence);
            ChangeLock(_defenceShortInfo != null && _defenceShortInfo.IsBuy == mySwitch.On);
        }

        private void ChangeLock(bool value = false)
        {
            StoreElementView.Apply(
                _panel,
                _lockDElementController.gameObject,
                _unlockDElementController.gameObject,
                (int)_defence,
                value,
                _lockDElementController,
                _unlockDElementController,
                _ammunitionsController);
        }
    }
}
