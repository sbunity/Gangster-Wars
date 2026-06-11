using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Spine.Unity;
using Spine;
using SBabchuk.Runtime.Architecture;
using Zenject;

namespace SBabchuk
{
    public class CollisionController : MonoBehaviour
    {
        [HideInInspector] public Transform _parent;
        private SignalBus _signalBus;

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

        [Inject]
        private void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
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
            _signalBus.Fire(new GrenadeDamageSignal(transform.position, damage, radius));

            transform.SetParent(_parent);

            this.gameObject.SetActive(false);

            transform.tag = "Collision";
        }
    }
}
