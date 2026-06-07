using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBabchuk
{
    public class SightController : MonoBehaviour
    {
        [HideInInspector]
        public static Vector2 sightPosition;

        [Header("Зміщення по У")]
        [Range(-2, 2)]
        public float offsetY;

        /// <summary>
        /// Awake
        /// </summary>
        void Awake()
        {
            sightPosition = transform.position;
        }

        /// <summary>
        /// Підписались на Енейбл
        /// </summary>
        public void OnEnable()
        {
            EasyTouch.On_TouchStart += OnTouchDown;
            EasyTouch.On_TouchDown += OnTouchMove;
            EasyTouch.On_TouchUp += OnTouchUp;
        }

        /// <summary>
        /// Підписались на Дізкйбл
        /// </summary>
        public void OnDisable()
        {
            EasyTouch.On_TouchStart -= OnTouchDown;
            EasyTouch.On_TouchDown -= OnTouchMove;
            EasyTouch.On_TouchUp -= OnTouchUp;
        }

        /// <summary>
        /// Рухаєм тачом
        /// </summary>
        /// <param name="gesture">Gesture.</param>
        virtual public void OnTouchMove(Gesture gesture)
        {
            Vector3 pos = gesture.GetTouchToWorldPoint(9f);
            transform.position = new Vector3(pos.x, pos.y + offsetY, 0);
            sightPosition = pos;
        }

        /// <summary>
        /// Нажаття на екрані
        /// </summary>
        /// <param name="gesture">Gesture.</param>
        public void OnTouchDown(Gesture gesture)
        {
            if (gesture.pickedObject)
            {
                if (gesture.pickedObject.tag == "FireZone" || gesture.pickedObject.tag == "Place" || gesture.pickedObject.tag == "Enemy")
                {
                    if (LeaderGangsterController.Instance)
                    {
                        LeaderGangsterController.Instance.Attack();
                    }
                }
            }
        }

        /// <summary>
        /// Віджаття
        /// </summary>
        public void OnTouchUp(Gesture gesture)
        {
            if (LeaderGangsterController.Instance)
            {
                LeaderGangsterController.Instance.StopAttack();
            }
        }
    }
}
