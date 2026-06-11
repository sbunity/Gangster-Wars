using System.Collections;
using System.Collections.Generic;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SBabchuk
{
    public class MainPlayerElementController : MonoBehaviour
    {
        [Header("Для якого персонажа")]
        public PersonagesName personage;

        [Header("Елементи при заблокуванні")]
        private MainPlayerLockElementController lockElementController;

        [Header("Елементи при розблокуванні")]
        private MainPlayerUnlockElementController unlockElementController;

        private AmmunitionsController ammunitionsController;

        private PersonageShortInfo personageShortInfo;
        private IPlayerProgressService _progressService;
        private SignalBus _signalBus;

        [Inject]
        private void Construct(
            IPlayerProgressService progressService,
            SignalBus signalBus)
        {
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

            ///Отримуєм LockElementController
            lockElementController = GetComponentInChildren<MainPlayerLockElementController>(true);

            ///Отримуєм UnlockElementController
            unlockElementController = GetComponentInChildren<MainPlayerUnlockElementController>(true);

            ///Отримуєм AmmunitionsController
            ammunitionsController = GetComponentInChildren<AmmunitionsController>(true);

            ///Перевірка на інтерактивність
            CheckInteractive();
        }

        /// <summary>
        /// Перевірка на інтерактивність
        /// </summary>
        private void CheckInteractive()
        {
            personageShortInfo = _progressService.GetPersonageShortInfo((int)personage);

            ChangeLock(personageShortInfo.isBuy == mySwitch.On);
        }

        /// <summary>
        /// Включаєм певні елементи 
        /// </summary>
        /// <param name="_value"></param>
        private void ChangeLock(bool _value = false)
        {
            lockElementController.gameObject.SetActive(!_value);

            unlockElementController.gameObject.SetActive(_value);

            if (lockElementController.gameObject.activeSelf)
            {
                lockElementController.Initialisation((int)personage);
            }

            if (unlockElementController.gameObject.activeSelf)
            {
                unlockElementController.Initialisation((int)personage);

                ammunitionsController.Initialisation((int)personage);
            }
        }
    }
}
