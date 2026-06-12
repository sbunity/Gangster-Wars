using System.Collections;
using System.Collections.Generic;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UnityEngine.Serialization;
using SBabchuk.Runtime.Databases.PlayerPrefs;
using SBabchuk.Runtime.UI.WeaponStore;

namespace SBabchuk.Runtime.UI.MainPlayer
{
    public class MainPlayerElementController : MonoBehaviour
    {
        [SerializeField, FormerlySerializedAs("personage")]
        private PersonagesName _personage;

        private MainPlayerLockElementController _lockElementController;
        private MainPlayerUnlockElementController _unlockElementController;
        private AmmunitionsController _ammunitionsController;
        private PersonageShortInfo _personageShortInfo;
        private IPlayerProgressService _progressService;
        private SignalBus _signalBus;

        [Inject]
        public void Construct(IPlayerProgressService progressService, SignalBus signalBus)
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
            _lockElementController = GetComponentInChildren<MainPlayerLockElementController>(true);
            _unlockElementController = GetComponentInChildren<MainPlayerUnlockElementController>(true);
            _ammunitionsController = GetComponentInChildren<AmmunitionsController>(true);
            CheckInteractive();
        }

        private void CheckInteractive()
        {
            _personageShortInfo = _progressService.GetPersonageShortInfo((int)_personage);
            ChangeLock(_personageShortInfo.IsBuy == mySwitch.On);
        }

        private void ChangeLock(bool _value = false)
        {
            _lockElementController.gameObject.SetActive(!_value);
            _unlockElementController.gameObject.SetActive(_value);
            if (_lockElementController.gameObject.activeSelf)
            {
                _lockElementController.Initialisation((int)_personage);
            }

            if (_unlockElementController.gameObject.activeSelf)
            {
                _unlockElementController.Initialisation((int)_personage);
                _ammunitionsController.Initialisation((int)_personage);
            }
        }
    }
}
