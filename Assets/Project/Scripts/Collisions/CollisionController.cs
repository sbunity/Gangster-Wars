using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Spine.Unity;
using Spine;

namespace SBabchuk
{
    public class CollisionController : MonoBehaviour
    {
        /// <summary>
        /// Нанесення урона
        /// </summary>
        /// <param name="_damage">Урон</param>
        /// <param name="_position">Позиція зриву</param>
        /// <param name="_radius">Радіус дії</param>
        public delegate void DealingDamage(Vector3 _position, int _damage, float _radius);
        public static event DealingDamage OnDealingDamage;

        [HideInInspector] public Transform _parent;

        /// <summary>
        /// Урон, що нанесе
        /// </summary>
        [HideInInspector] public int damage;

        /// <summary>
        /// Радіус нанесення урона
        /// </summary>
        [HideInInspector] public float radius;

        /// <summary>
        /// Час дії
        /// </summary>
        [HideInInspector] public float time;

        /// <summary>
        /// Предастартова ініціалізація
        /// </summary>
        public virtual void Awake()
        {
            
        }

        /// <summary>
        /// Старт
        /// </summary>
        public virtual void Start()
        {
            Subscribe();

            _parent = transform.parent;
        }

        /// <summary>
        /// Стратова ініціалізація
        /// </summary>
        public virtual void Subscribe()
        {
        }

        public virtual void Init(Vector3 _position, int _damage = 0, float _radius = 00, float _time = 0)
        {
            this.gameObject.SetActive(true);

            transform.position = _position;

            damage = _damage;

            radius = _radius;

            time = _time;
        }

        /// <summary>
        /// Перевіряєм на закінчення анімації
        /// </summary>
        /// <param name="trackEntry"></param>
        private void OnCompleteAnimation(TrackEntry trackEntry)
        {
            if (trackEntry.Animation.Name == "Fx2")
            {
                Pop();
            }
        }

        /// <summary>
		/// Повернення в пуш
		/// </summary>
		public void Pop()
        {
            OnDealingDamage?.Invoke(transform.position, damage, radius);

            transform.SetParent(_parent);

            this.gameObject.SetActive(false);

            transform.tag = "Collision";
        }
    }
}
