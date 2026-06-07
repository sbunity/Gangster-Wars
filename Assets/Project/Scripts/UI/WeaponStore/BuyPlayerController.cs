using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBabchuk
{
    public class BuyPlayerController : MonoBehaviour
    {
        public PersonagesName personage;

        private PersonageShortInfo personageShortInfo;

        private Personage personageInfo;

        [Header("Елементи при розблокуванні")]
        public GameObject unlockElement;

        [Header("Елементи при заблокуванні")]
        public GameObject lockElement;

        [Header("UI панелька")]
        public UIPanelController panel;

        Collider2D coll;

        /// <summary>
        /// Встановити підписку на евенти
        /// </summary>
        void OnEnable()
        {
            EasyTouch.On_TouchUp += OnTouchUp;
            PlayerPrefsDatabase.OnUpgraded += CheckInteractive;
            UIPanelController.OnReplace += Replace;
        }

        /// <summary>
        /// Втратити підписку на евенти
        /// </summary>
        void OnDisable()
        {
            EasyTouch.On_TouchUp -= OnTouchUp;
            PlayerPrefsDatabase.OnUpgraded -= CheckInteractive;
            UIPanelController.OnReplace -= Replace;
        }

        private void Start()
        {
            coll = GetComponent<Collider2D>();

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
            unlockElement.SetActive(_value);

            lockElement.SetActive(!_value);
        }

        private void OnTouchUp(Gesture gesture)
        {
            if (gesture.pickedObject == gameObject)
            {
                coll.enabled = false;

                panel.Show(panel);
            }
        }

        private void Replace(UIPanelController _panel)
        {
            if(_panel != panel)
                coll.enabled = true;
        }

        public void Hide()
        {
            panel.Hide(panel);

            coll.enabled = true;
        }

        public void Buy()
        {
            //panel.Hide(panel);

            PersistableSO.Instance.PlayerPrefs.BuyPersonage((int)personage);
        }
    }
}
