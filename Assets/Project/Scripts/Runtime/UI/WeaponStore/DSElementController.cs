using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UnityEngine.Serialization;

namespace SBabchuk
{
    public class DSElementController : MonoBehaviour
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
            _unlockDElementController = GetComponentInChildren<UnlockDElementController>(true);
            _lockDElementController = GetComponentInChildren<LockDElementController>(true);
            var defenceStore = _assetProvider.DefenseStoreDatabase;
            _defenceInfo = defenceStore.GetDefense((int)_defence);
            _ammunitionsController = GetComponentInChildren<AmmunitionsController>(true);
            _icon.sprite = _defenceInfo.Icon;
            _text.text = _defenceInfo.Name;
            CheckInteractive();
        }

        private void CheckInteractive()
        {
            _defenceShortInfo = _progressService.GetDefenceShortInfo((int)_defence);
            ChangeLock(_defenceShortInfo.IsBuy == mySwitch.On);
        }

        private void ChangeLock(bool _value = false)
        {
            _panel.Change(_value);
            _lockDElementController.gameObject.SetActive(!_value);
            _unlockDElementController.gameObject.SetActive(_value);
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
