using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SBabchuk
{
    public class DSElementController : MonoBehaviour
    {
        [Header("Для якої перепони")]
        public DefencesName defence;

        [Header("Іконка")]
        public Image ico;

        [Header("Назва")]
        public Text txt;

        [Header("Компоненти, які змінюють вигляд")]
        private SpriteSwap panel;

        [Header("Елементи при розблокуванні")]
        private UnlockDElementController unlockDElementController;

        [Header("Елементи при заблокуванні")]
        private LockDElementController lockDElementController;

        private AmmunitionsController ammunitionsController;

        private DefenceShortInfo defenceShortInfo;

        private Defense defenceInfo;

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
            unlockDElementController = GetComponentInChildren<UnlockDElementController>(true);

            ///Отримуєм LockElementController
            lockDElementController = GetComponentInChildren<LockDElementController>(true);

            ///Отримуєм зброю з бази
            defenceInfo = PersistableSO.Instance.DefenceStore.GetDefense((int)defence);

            ///Отримуєм AmmunitionsController
            ammunitionsController = GetComponentInChildren<AmmunitionsController>(true);

            ///Іконка
            ico.sprite = defenceInfo.ico;

            ///Ім*я
            txt.text = defenceInfo.name;

            ///Перевірка на інтерактивність
            CheckInteractive();
        }

        /// <summary>
        /// Перевірка на інтерактивність
        /// </summary>
        private void CheckInteractive()
        {
            defenceShortInfo = PersistableSO.Instance.PlayerPrefs.PlayerPrefs.GetDefenceShortInfo((int)defence);

            ChangeLock(defenceShortInfo.isBuy == mySwitch.On);
        }

        /// <summary>
        /// Включаєм певні елементи 
        /// </summary>
        /// <param name="_value"></param>
        private void ChangeLock(bool _value = false)
        {
            panel.Change(_value);

            lockDElementController.gameObject.SetActive(!_value);

            unlockDElementController.gameObject.SetActive(_value);

            if (lockDElementController.gameObject.activeSelf)
                lockDElementController.Initialisation((int)defence);

            if (unlockDElementController.gameObject.activeSelf)
            {
                unlockDElementController.Initialisation((int)defence);

                ammunitionsController.Initialisation((int)defence);
            }
        }
    }
}
