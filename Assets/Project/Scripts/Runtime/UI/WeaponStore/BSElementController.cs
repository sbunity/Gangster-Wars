using System.Collections;
using System.Collections.Generic;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
namespace SBabchuk
{
    public class BSElementController : MonoBehaviour
    {
        [Header("Для якої гранати")]
        public GrenadesName grenade;

        [Header("Іконка")]
        public Image ico;

        [Header("Назва")]
        public Text txt;

        [Header("Кількість")]
        public Text count;

        [Header("Компоненти, які змінюють вигляд")]
        private SpriteSwap panel;

        [Header("Елементи при розблокуванні")]
        private UnlockGElementController unlockGElementController;

        [Header("Елементи при заблокуванні")]
        private LockGElementController lockGElementController;

        private GrenadeShortInfo grenadeShortInfo;

        private Grenade grenadeInfo;
        private IAssetProvider _assetProvider;
        private IPlayerProgressService _progressService;
        private SignalBus _signalBus;

        [Inject]
        private void Construct(
            IAssetProvider assetProvider,
            IPlayerProgressService progressService,
            SignalBus signalBus)
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
            ///Отримуєм SpriteSwap
            panel = GetComponentInChildren<SpriteSwap>();

            ///Отримуєм LockElementController
            unlockGElementController = GetComponentInChildren<UnlockGElementController>(true);

            ///Отримуєм LockElementController
            lockGElementController = GetComponentInChildren<LockGElementController>(true);

            ///Отримуєм зброю з бази
            var bombStore = _assetProvider.BombStoreDatabase;
            grenadeInfo = bombStore.GetGrenade((int)grenade);

            ico.sprite = grenadeInfo.ico;

            txt.text = grenadeInfo.name;

            ///Перевірка на інтерактивність
            CheckInteractive();
        }

        /// <summary>
        /// Перевірка на інтерактивність
        /// </summary>
        private void CheckInteractive()
        {
            grenadeShortInfo = _progressService.GetGrenadeShortInfo((int)grenade);

            count.text = grenadeShortInfo.count.ToString();

            ChangeLock(grenadeShortInfo.isBuy == mySwitch.On);
        }

        /// <summary>
        /// Включаєм певні елементи 
        /// </summary>
        /// <param name="_value"></param>
        private void ChangeLock(bool _value = false)
        {
            panel.Change(_value);

            lockGElementController.gameObject.SetActive(!_value);

            unlockGElementController.gameObject.SetActive(_value);

            if (unlockGElementController.gameObject.activeSelf)
            {
                unlockGElementController.Initialisation((int)grenade);
            }
        }
    }
}
