using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBabchuk
{ 
    public class PersonageEnableController : MonoBehaviour
    {
        [Header("Снайпер")]
        public GameObject sniper;

        [Header("Бомбер")]
        public GameObject bomber;

        private void Start()
        {
            PlayerPrefs playerPrefs = PersistableSO.Instance.PlayerPrefs.PlayerPrefs;

            sniper.SetActive(playerPrefs.personages[0].isBuy == mySwitch.On);

            bomber.SetActive(playerPrefs.personages[1].isBuy == mySwitch.On);
        }
    }
}
