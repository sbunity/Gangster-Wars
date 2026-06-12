using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;

namespace SBabchuk.Runtime.UI.Constructor
{
    public class CounterController : MonoBehaviour
    {
        [SerializeField, FormerlySerializedAs("counterTxt")]
        private Text _counterTxt;

        [SerializeField] private SettingsController _settingsController;
        private int _counter = 1;
        private int _enemyID;

        public void SetEnemyID(int enemyID)
        {
            _enemyID = enemyID;
        }

        public void SetCounter(int value)
        {
            _counter = Mathf.Max(value, 1);
            _counterTxt.text = _counter.ToString();
        }

        public void More()
        {
            SetCounter(_counter + 1);
        }

        public void Less()
        {
            SetCounter(_counter - 1);
        }

        public void Click()
        {
            if (_settingsController == null)
                _settingsController = GetComponentInParent<SettingsController>(true);

            _settingsController.CreateNewEnemy(_enemyID, _counter);
        }
    }
}
