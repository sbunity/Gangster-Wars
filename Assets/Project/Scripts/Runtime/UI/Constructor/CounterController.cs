using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SBabchuk
{
    public class CounterController : MonoBehaviour
    {
        [Header("Лічильник скільки добавляєм юнітів")]
        public Text counterTxt;

        [SerializeField] private SettingsController _settingsController;

        private int counter = 1;

        private int enemyID;

        public void SetEnemyID(int _enemyID)
        {
            enemyID = _enemyID;
        }

        public void SetCounter(int _value)
        {
            counter = Mathf.Max(_value, 1);

            counterTxt.text = counter.ToString();
        }

        public void More()
        {
            SetCounter(counter + 1);
        }

        public void Less()
        {
            SetCounter(counter - 1);
        }

        public void Click()
        {
            if (_settingsController == null)
                _settingsController = GetComponentInParent<SettingsController>(true);

            _settingsController.CreateNewEnemy(enemyID, counter);
        }
    }
}
