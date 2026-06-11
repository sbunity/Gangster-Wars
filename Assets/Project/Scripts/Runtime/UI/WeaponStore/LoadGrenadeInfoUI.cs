using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SBabchuk
{ 
    public class LoadGrenadeInfoUI : MonoBehaviour
    {
        [Header("Тип гранати")]
        public GrenadesName type;

        [Header("Іконка")]
        public Image ico;

        [Header("Кількість")]
        public Text count;

        public void Initialized(Sprite _ico, int _count)
        {
            count.text = _count.ToString();

            ico.sprite = _ico;
        }
    }
}
