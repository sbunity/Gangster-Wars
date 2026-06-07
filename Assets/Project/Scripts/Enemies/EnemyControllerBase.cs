using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.UI;

namespace SBabchuk
{
    public class EnemyControllerBase : MonoBehaviour
    {
        #region Events
        public delegate void Die(int _id);
        public static event Die OnDie;
        #endregion//Events

        [Header("Аніматор")]
        [HideInInspector] public EnemyAnimationControllerBase e_animation;

        [Header("Властивості")]
        //[HideInInspector]
        public Enemy properties;

        [Header("Шлях по якому рухається")]
        private string pathName;

        [Header("Колайдер для зіткнення")]
        private Collider2D collisionCollider;

        [Header("База даних")]
        private EnemyDatabase database;

        [Header("Чи рухається юніт")]
        private bool isMoving;

        [Header("Чи була перервана атака юніта")]
        [HideInInspector] public bool isAttacked;

        [Header("Чи мертвий юніт")]
        private bool isDie;

        [Header("Точка для атаки")]
        [HideInInspector]
        public Transform target;

        [Header("Центр персонажа")]
        [HideInInspector]
        public Center center;

        /// <summary>
        /// Чи є зіткнення
        /// </summary>
        private bool collided;

        /// <summary>
        /// Скільки діамантів за смерть дасть
        /// </summary>
        private int diamonts;

        /// <summary>
        /// Максимальна кількість життя
        /// </summary>
        private int maxHealth;

        private Tween twnAtack;

        private Tween twnMove;

        private Tween twnFire;

        /// <summary>
        /// Скільки ми маєм часу щоб дійти
        /// </summary>
        private float timeMove = 0;

        /// <summary>
        /// Скільки ми часу йшли
        /// </summary>
        private float timeMoving = 0;

        /// <summary>
        /// Відписки
        /// </summary>
        private void OnDisable()
        {
            CollisionController.OnDealingDamage -= DealingDamage;
        }

        /// <summary>
        /// Підписки
        /// </summary>
        private void OnEnable()
        {
            CollisionController.OnDealingDamage += DealingDamage;
        }

        /// <summary>
        /// Отриманння урона від гранат
        /// </summary>
        /// <param name="_position"></param>
        /// <param name="_damage"></param>
        /// <param name="_radius"></param>
        private void DealingDamage(Vector3 _position, int _damage, float _radius)
        {
            if (center != null)
            {
                if (Vector3.Distance(center.GetPosition(), _position) <= _radius)
                {
                    TakeDamage(_damage);
                }
            }
        }

        /// <summary>
        /// Предстартова ініціалізація
        /// </summary>
        private void Awake()
        {
            e_animation = GetComponent<EnemyAnimationControllerBase>(); //Отримуєм Аніматор скрипта

            collisionCollider = GetComponent<Collider2D>(); //Отримуєм полігон

            center = GetComponentInChildren<Center>();
        }


        /// <summary>
        /// Стратові налаштування
        /// </summary>
        public void Start()
        {
            database = PersistableSO.Instance.Enemy; //Загружаєм базу даних
        }

        /// <summary>
        /// Ініціалізація ворога
        /// </summary>
        public virtual void Init(EnemyOfWave _enemyOfWave, Transform spawnPoint, Transform targetPoint, int _changeCraft)
        {
            if (!gameObject.activeSelf)
                gameObject.SetActive(true); //Включаєм об*єкт

            timeMoving = 0; //Скільки ми часу йшли, щоб знати скільки ще йти

            collided = false; //зіткнень немає

            target = targetPoint;

            properties = new Enemy(PersistableSO.Instance.Enemy.GetEnemy(_enemyOfWave.enemyID)); //Знаходимо властивості

            properties.changeCraft = _changeCraft;

            timeMove = properties.speedMove;

            transform.position = spawnPoint.position; //Спавним в точці

            collisionCollider = GetComponent<Collider2D>(); //Добавляєм колайдер

            collisionCollider.enabled = true;

            collisionCollider.isTrigger = true;

            StartMove(target); //Стартуєм рух
        }

        /// <summary>
		/// Запускаєм рух
		/// </summary>
		public void StartMove(Transform _target)
        {
            isMoving = true; //Встановлюєм флажок, рухаємось
            twnMove = this.gameObject.transform.DOMoveX(_target.position.x, timeMove)
                .SetEase(Ease.Linear)
                .OnStart(()=> {
                    timeMoving = Time.time;
                });

            e_animation.SetAnimation(AnimationsName.Walk); //Переключаємось в анімацію ходьби
        }

        /// <summary>
		/// Зупиняєм рух
		/// </summary>
		public void StopMove()
        {
            Utils.StopTween(twnMove);

            timeMove -= Time.time - timeMoving;

            e_animation.SetAnimation(AnimationsName.Idle); //ПЕреключаємось в анімацію очікування

            isMoving = false; //Встановлюєм влажок
        }

        /// <summary>
		/// Відновлюєм рух
		/// </summary>
		public void ContinueMove()
        {
            StartMove(target); //Стартуєм рух
        }

        /// <summary>
        /// Повертаєм швидкість руху
        /// </summary>
        public void ContinueSpeedMove()
        {
           
        }

        /// <summary>
		/// Перевірка відстані
		/// </summary>
		public bool CheckDistance()
        {
            //по ординат і х
            return (Mathf.Abs(transform.position.x - target.position.x) < properties.radiusAtack);
        }

        /// <summary>
		/// Метод самої атаки
		/// </summary>
		public virtual void Attack()
        {
            isAttacked = true; //Зараз ми в стані атаки

            e_animation.SetAnimation(AnimationsName.Attack); //Переключаємось в анімацію атаки
        }

        /// <summary>
        /// Запускаєм нову атаку
        /// </summary>
        public virtual void Attacked()
        {
            WaitNextAttack(properties.speedAtack);
        }

        /// <summary>
		/// Запускаєм таймер, щоб повторити атаку
		/// </summary>
		public void WaitNextAttack(float _delay = -1)
        {
            isAttacked = false; //Якщо атака стартує з нуля, то поки не запуститься ми точно не в стані атаки

            twnAtack = DOVirtual.DelayedCall((_delay < 0) ? properties.speedAtack : _delay, () => { //Таймер наступної атаки
                if (!isDie) //Перевірка чи ще живий
                    Attack(); //Атака
            }, false)
            .OnStart(()=> 
            {
                if (!isDie) //Перевірка чи ще живий
                    e_animation.SetAnimation(AnimationsName.Idle);
            });
        }

        /// <summary>
		/// Нанесення шкоди
		/// </summary>
		public virtual void GiveDamage()
        {
            BarricadeController.Instance.TakeDamage(properties.damage); //Наносимо урон башні

            LevelController.Instance.SpawnCollision(7, target.position, null);
        }

        /// <summary>
        /// Перевірка умови на можливість отримання урона
        /// </summary>
        /// <param name="_color">Color.</param>
        public void TakeDamage(int _damage)
        {
            //e_animation.SetAnimation(Actions.Hit); //включаєм анімацію отримання урона

            GetDamage(_damage);
        }

        /// <summary>
        /// Отримання урона, тут релізовано без анімації отримання урона
        /// </summary>
        /// <param name="_damage"></param>
        private void GetDamage(int _damage)
        {
            if (properties.health - _damage <= 0) //Кількість життів менша 0, померає
            {
                collided = true;

                isDie = true; //встановлюєм параметр, що говорить про смерть

                properties.health = 0; //встановлюєм життя рівним 0

                UpdateHealthBar(0);

                if (collisionCollider != null)
                {
                    collisionCollider.isTrigger = false; //виключаєм трігер в колайдері

                    collisionCollider.enabled = false; ////виключаєм колайдер
                }

                StopAllTweens();

                e_animation.SetAnimation(AnimationsName.Death); //включаєм анімацію смерті
            }
            else //Кількість життів більше життя, просто отримуєм урон
            {
                properties.health -= _damage; //зменшуєм кількість життів

                UpdateHealthBar((float)properties.health / maxHealth);
            }
        }

        /// <summary>
        /// Зупинаяєм всі твіни
        /// </summary>
        public virtual void StopAllTweens()
        {
            Utils.StopTween(twnAtack); //Вбиваєм твін, навсякий випадок, щоб не запустив анімацію атаки

            Utils.StopTween(twnMove); //Вбиваєм твін, навсякий випадок, щоб не запустив анімацію руху

            Utils.StopTween(twnFire);
        }

        /// <summary>
		/// При завершенні анімації смерті
		/// </summary>
		public void Dead()
        {
            OnDie?.Invoke(properties.id); //Оголошуєм, про нашу смерть

            PersistableSO.Instance.PlayerPrefs.SetCoin(properties.gold);

            CheckSpawnBonus(); //Чи варто після смерті юніта бонус створювати

            Pop(); //Повертаємось в пул
        }

        private void CheckSpawnBonus()
        {
            if (UnityEngine.Random.Range(0, 100) <= properties.changeCraft)
            {
                LevelController.Instance.SpawnBonus(this.gameObject.transform.position);
            }
        }

        /// <summary>
        /// Повернутись в пул
        /// </summary>
        public void Pop()
        {
            this.gameObject.SetActive(false); //Виключаєм об*єкт

            isDie = false;
        }

        /// <summary>
        /// Тругер на зіткнення з пулею
        /// </summary>
        /// <param name="other">Other.</param>
        public void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "BulletHero" && !collided) //Перевіряєм чи ми взаємодієм з пулею героя
            {
                BaseBulletController bullet = other.GetComponent<BaseBulletController>(); //Отримуєм Контроллер пулі

                bullet.Pop(); //Повертаєм пулю в пул

                TakeDamage(bullet.damage); //Отримуєм урон
            }
            else if (other.tag == "Grenade" && !collided) //Перевіряєм чи ми взаємодієм гранатою
            {
                GrenadeController grenade = other.GetComponent<GrenadeController>(); //Отримуєм Контроллер пулі

                if ((GrenadesName)grenade.properties.id == GrenadesName.Grenade_3)
                    grenade.Action(0); //Повертаєм пулю в пул
            }
            if (other.tag == "Fire") //Перевіряєм чи ми взаємодієм гранатою
            {
                twnFire = DOVirtual.DelayedCall(1, () =>
                {
                    TakeDamage(1);
                }).SetLoops(-1);

            }
        }

        public void OnTriggerExit2D(Collider2D other)
        {
            Utils.StopTween(twnFire);
        }

        /// <summary>
        /// /Оновляєм HealthBar
        /// </summary>
        /// <param name="_value"></param>
        public void UpdateHealthBar(float _value, float duration = 0.5f)
        {
            //if (healthbar)
             //   healthbar.DOFillAmount(_value, duration);
        }

        /// <summary>
        /// Апдейт
        /// </summary>
        void FixedUpdate()
        {
            if (isMoving) //Пeревірка чи ще рухаємось
            {
                if (CheckDistance()) //Перевірка на відстань
                {
                    StopMove(); //Зупиняєм рух

                    Attack(); //Атакуєм
                }
            }
        }

        //ТЕСТОВЕ ВИДАЛИТИ
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                TakeDamage(2);
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                StopMove();
                Attack();
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                ContinueMove();
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                StopMove();
            }
            if (Input.GetKeyDown(KeyCode.M))
            {
                StartMove(target);
            }
        }
    }
}
