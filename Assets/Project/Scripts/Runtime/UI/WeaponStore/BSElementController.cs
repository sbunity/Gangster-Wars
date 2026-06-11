using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;

namespace SBabchuk
{
    public class BSElementController : StoreElementControllerBase
    {
        [SerializeField, FormerlySerializedAs("grenade")]
        private GrenadesName _grenade;

        [SerializeField, FormerlySerializedAs("ico")]
        private Image _icon;

        [SerializeField, FormerlySerializedAs("txt")]
        private Text _text;

        [SerializeField, FormerlySerializedAs("count")]
        private Text _count;

        private SpriteSwap _panel;
        private UnlockGElementController _unlockGElementController;
        private LockGElementController _lockGElementController;
        private GrenadeShortInfo _grenadeShortInfo;
        private Grenade _grenadeInfo;

        private void Start()
        {
            _panel = GetComponentInChildren<SpriteSwap>();
            _unlockGElementController = GetComponentInChildren<UnlockGElementController>(true);
            _lockGElementController = GetComponentInChildren<LockGElementController>(true);
            var bombStore = AssetProvider.BombStoreDatabase;
            _grenadeInfo = bombStore.GetGrenade((int)_grenade);
            _icon.sprite = _grenadeInfo.Icon;
            _text.text = _grenadeInfo.Name;
            RefreshState();
        }

        protected override void RefreshState()
        {
            _grenadeShortInfo = ProgressService.GetGrenadeShortInfo((int)_grenade);
            _count.text = _grenadeShortInfo.Count.ToString();
            ChangeLock(_grenadeShortInfo.IsBuy == mySwitch.On);
        }

        private void ChangeLock(bool _value = false)
        {
            ApplyLockState(_panel, _lockGElementController.gameObject, _unlockGElementController.gameObject, _value);
            
            if (_unlockGElementController.gameObject.activeSelf)
            {
                _unlockGElementController.Initialisation((int)_grenade);
            }
        }
    }
}
