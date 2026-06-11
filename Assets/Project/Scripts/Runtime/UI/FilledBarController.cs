using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SBabchuk
{ 
    public class FilledBarController : MonoBehaviour
    {
        [Header("Елемент, що заповнюється")]
        public Slider slider;
        
        [Header("Швидкість заповнення")]
        [Range(0.1f, 3)]
        public float speed;

        private float currentvalue;

        Tween twn;

        public void UpdateFlled(float _value)
        {
            twn?.Kill();

            currentvalue = currentvalue + _value;

            twn = slider.DOValue(currentvalue, speed).SetUpdate(true);
        }
    }
}
