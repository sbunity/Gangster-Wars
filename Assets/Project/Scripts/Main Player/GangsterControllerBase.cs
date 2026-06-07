using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using DG.Tweening;

namespace SBabchuk
{
    public class GangsterControllerBase : MonoBehaviour
    {
        [Header("Аніматор")]
        [HideInInspector]
        public GangsterAnimationController e_animation;

        [Header("Точка де буде створена пуля")]
        [HideInInspector]
        public Center createBulletPoint;

        /// <summary>
        /// Тимчасовий твін
        /// </summary>
        [HideInInspector]
        public Tween twnAtack;

        /// Передстартова ініціалізація
        /// </summary>
		public virtual void Awake()
        {
            e_animation = GetComponentInChildren<GangsterAnimationController>(); //Отримуєм Аніматор скрипта

            createBulletPoint = GetComponentInChildren<Center>();
        }

        /// <summary>
        /// Стартова ініціалізація
        /// </summary>
        public virtual void Start()
        {
            Init(); //Ініціалізація героя
        }

        /// <summary>
        /// Ініціалізація героя
        /// </summary>
        /// <param name="_id">Id героя</param>
        public virtual void Init()
        {
            e_animation.SetAnimation(AnimationsName.Idle);
        }

        /// <summary>
        /// Метод атаки
        /// </summary>
        public virtual void Attack()
        {
            Debug.Log("Attack");
        }

        /// <summary>
        /// Метод, що викликається після закінчення анімації атаки
        /// </summary>
        public virtual void AttackEnded()
        {

        }

        /// <summary>
        /// Нанесення шкоди
        /// </summary>
        public virtual void SpawnBullet()
        {
            Spawn(createBulletPoint.GetPosition()); //Створюєм пулю
        }

        /// <summary>
        /// Створюєм пулю
        /// </summary>
        public virtual void Spawn(Vector3 _position)
        {
            BulletController _bullet = (GetPool(0).GetPooledObject()).GetComponent<BulletController>();

            _bullet.transform.tag = "BulletHero"; //Ставив тег, щоб знати що це пуля героя

            _bullet.Init(0, 4, _position); //Ініціалізація пулі
        }

        /// <summary>
        /// Отримуєм пулл по ID
        /// </summary>
        /// <param name="_bulletID"></param>
        /// <returns></returns>
        public virtual Pool GetPool(int _spellID)
        {
            return PoolManager.GetPoolByName("Bullet_" + (_spellID + 1));
        }

        public virtual void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                Attack();
            }
        }
    }
}
