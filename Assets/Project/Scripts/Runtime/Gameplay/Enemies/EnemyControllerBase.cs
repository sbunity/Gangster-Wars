using UnityEngine;
using DG.Tweening;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Gameplay.Enemies;
using SBabchuk.Runtime.Services.Contracts;
using Zenject;

namespace SBabchuk
{
    public class EnemyControllerBase : MonoBehaviour
    {
        [HideInInspector] public EnemyAnimationControllerBase e_animation;

        public Enemy properties;

        // Kept public for subclasses that drive their own attack animation (e.g. BigBoss).
        [HideInInspector] public bool isAttacked;

        [HideInInspector] public Transform target;

        [HideInInspector] public Center center;

        private bool collided;
        private bool isDie;

        private Tween twnFire;

        private IAssetProvider _assetProvider;
        private IPlayerProgressService _progressService;
        private IGameFactory _gameFactory;
        private ICombatService _combatService;
        private ILevelRuntimeService _levelRuntimeService;
        private BarricadeController _barricadeController;
        private SignalBus _signalBus;
        private bool _isSubscribedToSignals;

        private EnemyView _view;
        private EnemyHealth _health;
        private EnemyMovement _movement;
        private EnemyAttack _attack;
        private EnemyDeath _death;
        private EnemyReward _reward;

        protected ILevelRuntimeService LevelRuntimeService => _levelRuntimeService;

        [Inject]
        private void Construct(
            IAssetProvider assetProvider,
            IPlayerProgressService progressService,
            IGameFactory gameFactory,
            ICombatService combatService,
            ILevelRuntimeService levelRuntimeService,
            BarricadeController barricadeController,
            SignalBus signalBus)
        {
            _assetProvider = assetProvider;
            _progressService = progressService;
            _gameFactory = gameFactory;
            _combatService = combatService;
            _levelRuntimeService = levelRuntimeService;
            _barricadeController = barricadeController;
            _signalBus = signalBus;

            SubscribeSignals();
        }

        private void OnDisable()
        {
            UnsubscribeSignals();

            StopAllTweens();
        }

        private void OnEnable()
        {
            SubscribeSignals();
        }

        private void DealingDamage(GrenadeDamageSignal signal)
        {
            DealingDamage(signal.Position, signal.Damage, signal.Radius);
        }

        private void DealingDamage(Vector3 _position, int _damage, float _radius)
        {
            if (center != null)
            {
                if (Vector2.Distance(center.GetPosition(), _position) <= _radius)
                {
                    TakeDamage(_damage);
                }
            }
        }

        private void Awake()
        {
            _view = GetOrAdd<EnemyView>();
            _health = GetOrAdd<EnemyHealth>();
            _movement = GetOrAdd<EnemyMovement>();
            _attack = GetOrAdd<EnemyAttack>();
            _death = GetOrAdd<EnemyDeath>();
            _reward = GetOrAdd<EnemyReward>();

            _view.Initialize();

            e_animation = _view.Animation;

            center = _view.Center;

            _health.Changed += OnHealthChanged;
            _health.Died += OnHealthDied;
        }

        public virtual void Init(EnemyOfWave _enemyOfWave, Transform spawnPoint, Transform targetPoint, int _changeCraft)
        {
            StopAllTweens();

            collided = false;
            isAttacked = false;
            isDie = false;

            target = targetPoint;

            var enemyDatabase = _assetProvider.EnemyDatabase;
            properties = new Enemy(enemyDatabase.GetEnemy(_enemyOfWave.enemyID));
            properties.changeCraft = _changeCraft;

            _health.Initialize(properties.health);
            _movement.Initialize(_view, target, properties.speedMove);
            _attack.Initialize(_view);

            transform.position = spawnPoint.position;

            if (!gameObject.activeSelf)
                gameObject.SetActive(true);

            _view.SetColliderEnabled(true);

            StartMove(target);
        }

        public void StartMove(Transform _target)
        {
            _movement.Move();
        }

        public void StopMove()
        {
            _movement.Stop();
            _view.SetAnimation(AnimationsName.Idle);
        }

        public void ContinueMove()
        {
            StartMove(target);
        }

        public void ContinueSpeedMove()
        {
        }

        public bool CheckDistance()
        {
            return _movement.IsInAttackRange(properties.radiusAtack);
        }

        public virtual void Attack()
        {
            isAttacked = true;
            _attack.Attack();
        }

        public virtual void Attacked()
        {
            WaitNextAttack(properties.speedAtack);
        }

        public void WaitNextAttack(float _delay = -1)
        {
            isAttacked = false;

            var delay = (_delay < 0) ? properties.speedAtack : _delay;
            _attack.WaitNext(delay, Attack);
        }

        public virtual void GiveDamage()
        {
            if (_barricadeController != null)
                _barricadeController.TakeDamage(properties.damage);

            if (_gameFactory != null)
                _gameFactory.CreateCollision(7, target.position);
            else if (_levelRuntimeService != null)
                _levelRuntimeService.SpawnCollision(7, target.position, null);
        }

        public void TakeDamage(int _damage)
        {
            _health.ApplyDamage(_damage);
        }

        private void OnHealthChanged(float normalized)
        {
            if (properties != null)
                properties.health = _health.Current;
        }

        private void OnHealthDied()
        {
            collided = true;
            isDie = true;
            _attack.MarkDead();
            StopAllTweens();
            _death.Play(_view);
        }

        public virtual void StopAllTweens()
        {
            if (_attack != null)
                _attack.Stop();

            if (_movement != null)
                _movement.Stop();

            twnFire?.Kill();
        }

        /// <summary>
        /// Triggered by the Spine "Death" animation completion event.
        /// </summary>
        public void Dead()
        {
            _signalBus.Fire(new EnemyDiedSignal(properties.id));

            _reward.Grant(properties);

            CheckSpawnBonus();

            Pop();
        }

        private void CheckSpawnBonus()
        {
            if (UnityEngine.Random.Range(0, 100) <= properties.changeCraft)
            {
                if (_levelRuntimeService != null)
                    _levelRuntimeService.SpawnBonus(this.gameObject.transform.position);
            }
        }

        private void SubscribeSignals()
        {
            if (_isSubscribedToSignals || _signalBus == null)
                return;

            _signalBus.Subscribe<GrenadeDamageSignal>(DealingDamage);
            _isSubscribedToSignals = true;
        }

        private void UnsubscribeSignals()
        {
            if (!_isSubscribedToSignals || _signalBus == null)
                return;

            _signalBus.Unsubscribe<GrenadeDamageSignal>(DealingDamage);
            _isSubscribedToSignals = false;
        }

        public void Pop()
        {
            this.gameObject.SetActive(false);

            isDie = false;
        }

        public void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "BulletHero" && !collided)
            {
                BaseBulletController bullet = other.GetComponent<BaseBulletController>();

                if (_combatService != null)
                    _combatService.ApplyBulletHit(this, bullet);
                else
                {
                    bullet.Pop();
                    TakeDamage(bullet.damage);
                }
            }
            else if (other.tag == "Grenade" && !collided)
            {
                GrenadeController grenade = other.GetComponent<GrenadeController>();

                if ((GrenadesName)grenade.properties.id == GrenadesName.Grenade_3)
                    grenade.Action(0);
            }
            if (other.tag == "Fire")
            {
                twnFire = DOVirtual.DelayedCall(1, () =>
                {
                    TakeDamage(1);
                }).SetLoops(-1);
            }
        }

        public void OnTriggerExit2D(Collider2D other)
        {
            twnFire?.Kill();
        }

        private void FixedUpdate()
        {
            if (_movement.IsMoving)
            {
                if (CheckDistance())
                {
                    StopMove();

                    Attack();
                }
            }
        }

        private T GetOrAdd<T>() where T : Component
        {
            var component = GetComponent<T>();
            return component != null ? component : gameObject.AddComponent<T>();
        }
    }
}
