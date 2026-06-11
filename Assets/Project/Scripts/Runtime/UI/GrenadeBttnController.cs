using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using SBabchuk.Runtime.Services.Contracts;
using Zenject;

namespace SBabchuk
{
    public class GrenadeBttnController : MonoBehaviour
    {
        [Header("За яку гранату відповідає")]
        public GrenadesName grenadesName; //ідентифікатор стікера

        [Header("Іконка")]
        public Image ico;

        /// <summary>
        /// Image - компонент кнопки
        /// </summary>
        private Image imgBttn;
        private IHandService _handService;

        [Inject]
        private void Construct(IHandService handService)
        {
            _handService = handService;
        }

        /// <summary>
        /// Предстартова ініціалізація
        /// </summary>
        private void Awake()
        {
            imgBttn = GetComponent<Image>();
        }

        /// <summary>
        /// Подія нажаття на кнопку UI
        /// </summary>
        public void PointerDown()
        {
            CheckIco(false);

            //OnPointerDown?.Invoke(false);

            _handService?.Init(grenadesName, this);
        }

        /// <summary>
        /// Іконка включається і виключається
        /// </summary>
        /// <param name="_value"></param>
        public void CheckIco(bool _value)
        {
            if (ico.enabled != _value)
            {
                ico.enabled = _value;
            }

            imgBttn.raycastTarget = _value;
        }
    }
}
