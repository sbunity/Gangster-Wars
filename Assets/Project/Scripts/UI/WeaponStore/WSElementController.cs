using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SBabchuk
{
    public class WSElementController : MonoBehaviour
    {
        [Header("Для якої зброї")]
        public WeaponsName weapon;

        [Header("Іконка")]
        public Image ico;

        [Header("Назва")]
        public Text txt;

        [Header("Компоненти, які змінюють вигляд")]
        private SpriteSwap panel;

        [Header("Елементи при заблокуванні")]
        private LockElementController lockElementController;

        [Header("Елементи при розблокуванні")]
        private UnlockElementController unlockElementController;

        private AmmunitionsController ammunitionsController;

        private WeaponShortInfo weaponShortInfo;

        private Weapon weaponInfo;

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
            ///Отримуєм SpriteSwap
            panel = GetComponentInChildren<SpriteSwap>();

            ///Отримуєм LockElementController
            lockElementController = GetComponentInChildren<LockElementController>(true);

            ///Отримуєм UnlockElementController
            unlockElementController = GetComponentInChildren<UnlockElementController>(true);

            ///Отримуєм AmmunitionsController
            ammunitionsController = GetComponentInChildren<AmmunitionsController>(true);

            ///Отримуєм зброю з бази
            weaponInfo = PersistableSO.Instance.WeaponStore.GetWeapon((int)weapon);

            ico.sprite = weaponInfo.ico;

            txt.text = weaponInfo.name;

            ///Перевірка на інтерактивність
            CheckInteractive();
        }

        /// <summary>
        /// Перевірка на інтерактивність
        /// </summary>
        private void CheckInteractive()
        {
            weaponShortInfo = PersistableSO.Instance.PlayerPrefs.PlayerPrefs.GetWeaponShortInfo((int)weapon);

            ChangeLock(weaponShortInfo.isBuy == mySwitch.On);
        }

        /// <summary>
        /// Включаєм певні елементи 
        /// </summary>
        /// <param name="_value"></param>
        private void ChangeLock(bool _value = false)
        {
            panel.Change(_value);

            lockElementController.gameObject.SetActive(!_value);

            unlockElementController.gameObject.SetActive(_value);

            if (lockElementController.gameObject.activeSelf)
                lockElementController.Initialisation((int)weapon);

            if (unlockElementController.gameObject.activeSelf)
            {
                unlockElementController.Initialisation((int)weapon);

                ammunitionsController.Initialisation((int)weapon);
            }
        }
    }
}
