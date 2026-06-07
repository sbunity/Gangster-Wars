using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SBabchuk
{
    public class PatronController : MonoBehaviour
    {
        [Header("Індекс патрона")]
        public int index;

        [Header("Спрайд повного патрона")]
        public Sprite fullSprite;

        [Header("Спрайд порожнього патрона")]
        public Sprite emptySprite;

        [Header("Іконка")]
        Image ico;

        private void OnEnable()
        {
            LeaderGangsterController.OnInitMagazine += Init;
            LeaderGangsterController.OnInitPatrons += Check;
            LeaderGangsterController.OnUpdateCountPatrons += Check;
        }

        private void OnDisable()
        {
            LeaderGangsterController.OnInitMagazine -= Init;
            LeaderGangsterController.OnInitPatrons -= Check;
            LeaderGangsterController.OnUpdateCountPatrons -= Check;
        }

        private void Awake()
        {
            ico = GetComponentInChildren<Image>();
        }

        private void Init(int _value)
        {
            ico.gameObject.SetActive(index <= _value);

            //Check(_value);
        }

        private void Check(int _value)
        {
            ico.sprite = (index <= _value) ? fullSprite : emptySprite;
        }

    }
}
