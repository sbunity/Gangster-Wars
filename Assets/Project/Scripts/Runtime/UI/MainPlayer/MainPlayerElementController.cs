using SBabchuk.Runtime.Databases.PlayerPrefs;
using SBabchuk.Runtime.UI.WeaponStore;
using UnityEngine;
using UnityEngine.Serialization;

namespace SBabchuk.Runtime.UI.MainPlayer
{
    public class MainPlayerElementController : StoreElementControllerBase
    {
        [SerializeField, FormerlySerializedAs("personage")]
        private PersonagesName _personage;

        private MainPlayerLockElementController _lockElementController;
        private MainPlayerUnlockElementController _unlockElementController;
        private AmmunitionsController _ammunitionsController;
        private PersonageShortInfo _personageShortInfo;

        protected override void CacheView()
        {
            _lockElementController = GetComponentInChildren<MainPlayerLockElementController>(true);
            _unlockElementController = GetComponentInChildren<MainPlayerUnlockElementController>(true);
            _ammunitionsController = GetComponentInChildren<AmmunitionsController>(true);
        }

        protected override void RefreshState()
        {
            _personageShortInfo = ProgressService.GetPersonageShortInfo((int)_personage);
            ChangeLock(_personageShortInfo != null && _personageShortInfo.IsBuy == mySwitch.On);
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
