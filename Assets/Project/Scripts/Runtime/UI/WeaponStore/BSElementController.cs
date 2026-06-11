using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UnityEngine.Serialization;

namespace SBabchuk
{
    public class BSElementController : MonoBehaviour
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
            _unlockGElementController = GetComponentInChildren<UnlockGElementController>(true);
            _lockGElementController = GetComponentInChildren<LockGElementController>(true);
            var bombStore = _assetProvider.BombStoreDatabase;
            _grenadeInfo = bombStore.GetGrenade((int)_grenade);
            _icon.sprite = _grenadeInfo.Icon;
            _text.text = _grenadeInfo.Name;
            CheckInteractive();
        }

        private void CheckInteractive()
        {
            _grenadeShortInfo = _progressService.GetGrenadeShortInfo((int)_grenade);
            _count.text = _grenadeShortInfo.Count.ToString();
            ChangeLock(_grenadeShortInfo.IsBuy == mySwitch.On);
        }

        private void ChangeLock(bool _value = false)
        {
            _panel.Change(_value);
            _lockGElementController.gameObject.SetActive(!_value);
            _unlockGElementController.gameObject.SetActive(_value);
            
            if (_unlockGElementController.gameObject.activeSelf)
            {
                _unlockGElementController.Initialisation((int)_grenade);
            }
        }
    }
}
