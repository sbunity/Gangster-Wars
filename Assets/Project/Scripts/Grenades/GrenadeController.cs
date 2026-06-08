using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Spine.Unity;

namespace SBabchuk
{
    public class GrenadeController : MonoBehaviour
    {
        public delegate void DealingDamage(Vector3 _position, int _damage, float _radius);
        public static event DealingDamage OnDealingDamage;
        
        [HideInInspector]
        public Transform _parent;

        [Header("Колайдер для зіткнення")]
        private Collider2D collisionCollider;

        [HideInInspector]
        public Grenade properties;

        private Tween twn;

        /// <summary>
        /// Стартова ініціазізація
        /// </summary>
        void Start()
        {
            _parent = transform.parent;
        }

        /// <summary>
        /// Ініціалізація
        /// </summary>
        /// <param name="_id"></param>
        /// <param name="_damage"></param>
        /// <param name="_position"></param>
        /// <param name="_target"></param>
        public void Init(int _id = 0, int _damage = 0, Vector3 _position = default(Vector3), Transform _target = null)
        {
            this.gameObject.SetActive(true);

            transform.position = _position;

            properties = PersistableSO.Instance.BombStore.GetGrenade(_id);

            if (collisionCollider == null)
            {
                collisionCollider = gameObject.AddComponent<PolygonCollider2D>();
            }

            collisionCollider.isTrigger = false;

            Action(properties.delay);
        }

       /// <summary>
       /// Запускаєм дію
       /// </summary>
        public void Action(float _value)
        {
            Timer(_value);
        }

        /// <summary>
        /// Таймер
        /// </summary>
        /// <param name="_delay">Затримка до зриву</param>
        public void Timer(float _delay)
        {
            Utils.StopTween(twn);

            twn = DOVirtual.DelayedCall(_delay, ()=> {
                Explosion();
            });
        }

        /// <summary>
        /// Добавлення ефекта зриву
        /// </summary>
        public void Explosion()
        {
            Pop();

            LevelController.Instance.SpawnCollision((int)properties.collision, transform.position, properties);
        }

        /// <summary>
		/// Повернення в пуш
		/// </summary>
		public void Pop()
        {
            OnDealingDamage?.Invoke(transform.position, properties.damage, properties.radius);
            
            transform.SetParent(_parent);
            
            collisionCollider.isTrigger = false;
            
            this.gameObject.SetActive(false);

            transform.tag = "Grenade";
        }
    }
}
