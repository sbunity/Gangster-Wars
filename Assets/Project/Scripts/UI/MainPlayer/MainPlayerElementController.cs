using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

        private void OnEnable()
        {
            PlayerPrefsDatabase.OnUpgraded += CheckInteractive;
        }

        private void OnDisable()
        {
            PlayerPrefsDatabase.OnUpgraded -= CheckInteractive;
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
            personageShortInfo = PersistableSO.Instance.PlayerPrefs.PlayerPrefs.GetPersonageShortInfo((int)personage);

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
