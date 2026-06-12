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
        private SignalSubscriptions _signals;

        [Inject]
        public void Construct(IPlayerProgressService progressService, SignalBus signalBus)
        {
            _progressService = progressService;
            _signals = new SignalSubscriptions(signalBus)
                .Add<ProgressUpgradedSignal>(OnProgressUpgraded);
        }

        private void OnEnable() => _signals?.Enable();

        private void OnDisable() => _signals?.Disable();

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

        private void ChangeLock(bool value = false)
        {
            StoreElementView.Apply(
                null,
                _lockElementController.gameObject,
                _unlockElementController.gameObject,
                (int)_personage,
                value,
                _lockElementController,
                _unlockElementController,
                _ammunitionsController);
        }
    }
}
