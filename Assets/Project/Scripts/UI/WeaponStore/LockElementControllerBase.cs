using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SBabchuk
{
    public class LockElementControllerBase : MonoBehaviour
    {
        [Header("Поле для виведення вартості покупки зброї")]
        public Text priceBuy;

        [Header("Кнопка покупки")]
        public Button bttnBuy;

        [HideInInspector]
        public int id;

        /// <summary>
        /// Ініціалізація
        /// </summary>
        /// <param name="_id"></param>
        public virtual void Initialisation(int _id)
        {
           
        }

        /// <summary>
        /// Поукупка зброї
        /// </summary>
        public virtual void Buy()
        {
        }
    }
}
