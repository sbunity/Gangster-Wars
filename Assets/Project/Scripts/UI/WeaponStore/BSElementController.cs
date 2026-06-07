using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
            unlockGElementController = GetComponentInChildren<UnlockGElementController>(true);

            ///Отримуєм LockElementController
            lockGElementController = GetComponentInChildren<LockGElementController>(true);

            ///Отримуєм зброю з бази
            grenadeInfo = PersistableSO.Instance.BombStore.GetGrenade((int)grenade);

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
            grenadeShortInfo = PersistableSO.Instance.PlayerPrefs.PlayerPrefs.GetGrenadeShortInfo((int)grenade);

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
