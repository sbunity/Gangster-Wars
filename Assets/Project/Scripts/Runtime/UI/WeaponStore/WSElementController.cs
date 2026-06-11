using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UnityEngine.Serialization;

namespace SBabchuk
{
    public class WSElementController : MonoBehaviour
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
        private IAssetProvider _assetProvider;
        private IPlayerProgressService _progressService;
        private SignalBus _signalBus;

        [Inject]
        public void Construct(IAssetProvider assetProvider, IPlayerProgressService progressService, SignalBus signalBus)
        {
            _assetProvider = assetProvider;
            _progressService = progressService;
            _signalBus = signalBus;
        }

        private void OnEnable()
        {
            _signalBus?.Subscribe<ProgressUpgradedSignal>(OnProgressUpgraded);
        }

        private void OnDisable()
        {
            _signalBus?.Unsubscribe<ProgressUpgradedSignal>(OnProgressUpgraded);
        }

        private void OnProgressUpgraded(ProgressUpgradedSignal signal)
        {
            CheckInteractive();
        }

        private void Start()
        {
            _panel = GetComponentInChildren<SpriteSwap>();
            _lockElementController = GetComponentInChildren<LockElementController>(true);
            _unlockElementController = GetComponentInChildren<UnlockElementController>(true);
            _ammunitionsController = GetComponentInChildren<AmmunitionsController>(true);
            var weaponStore = _assetProvider.WeaponStoreDatabase;
            _weaponInfo = weaponStore.GetWeapon((int)_weapon);
            _icon.sprite = _weaponInfo.Icon;
            _text.text = _weaponInfo.Name;
            CheckInteractive();
        }

        private void CheckInteractive()
        {
            _weaponShortInfo = _progressService.GetWeaponShortInfo((int)_weapon);
            ChangeLock(_weaponShortInfo.IsBuy == mySwitch.On);
        }

        private void ChangeLock(bool _value = false)
        {
            _panel.Change(_value);
            _lockElementController.gameObject.SetActive(!_value);
            _unlockElementController.gameObject.SetActive(_value);
            
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
