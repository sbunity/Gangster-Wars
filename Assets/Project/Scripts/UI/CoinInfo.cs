using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SBabchuk
{
    public class CoinInfo : MonoBehaviour
    {
        [Header("Текстове поле для виведення")]
        private Text txt;

        private void OnEnable()
        {
            PlayerPrefsDatabase.OnChangeCoin += UpdateCoin;
        }

        private void OnDisable()
        {
            PlayerPrefsDatabase.OnChangeCoin -= UpdateCoin;
        }

        private void Awake()
        {
            txt = GetComponentInChildren<Text>();
        }

        private void Start()
        {
            UpdateCoin();
            
        }

        private void UpdateCoin()
        {
            txt.text = PersistableSO.Instance.PlayerPrefs.PlayerPrefs.coin.ToString();
        }
    }
}
