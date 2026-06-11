using UnityEngine;
using DG.Tweening;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Gameplay.Enemies;
using SBabchuk.Runtime.Services.Contracts;
using Zenject;
using UnityEngine.Serialization;

namespace SBabchuk
{
    public class EnemyControllerBase : MonoBehaviour
    {
        [SerializeField, FormerlySerializedAs("e_animation")]
        private EnemyAnimationControllerBase _animation;
        public EnemyAnimationControllerBase Animation { get => _animation; private set => _animation = value; }

        [SerializeField, FormerlySerializedAs("properties")]
        private Enemy _properties;
        public Enemy Properties { get => _properties; private set => _properties = value; }

        [SerializeField, FormerlySerializedAs("isAttacked")]
        private bool _isAttacked;
        public bool IsAttacked { get => _isAttacked; protected set => _isAttacked = value; }

        [SerializeField, FormerlySerializedAs("target")]
        private Transform _target;
        public Transform Target { get => _target; private set => _target = value; }

        [SerializeField, FormerlySerializedAs("center")]
        private Center _center;
        public Center Center { get => _center; private set => _center = value; }

        private bool _collided;
        private bool _isDie;
        private Tween _twnFire;
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
        public void Construct(IAssetProvider assetProvider, IPlayerProgressService progressService, IGameFactory gameFactory, ICombatService combatService, ILevelRuntimeService levelRuntimeService, BarricadeController barricadeController, SignalBus signalBus)
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
            => DealingDamage(signal.Position, signal.Damage, signal.Radius);

        private void DealingDamage(Vector3 _position, int _damage, float _radius)
        {
            if (_center != null)
            {
                if (Vector2.Distance(_center.GetPosition(), _position) <= _radius)
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
            _animation = _view.Animation;
            _center = _view.Center;
            _health.Changed += OnHealthChanged;
            _health.Died += OnHealthDied;
        }

        public virtual void Init(EnemyOfWave _enemyOfWave, Transform spawnPoint, Transform targetPoint, int _changeCraft)
        {
            StopAllTweens();
            _collided = false;
            _isAttacked = false;
            _isDie = false;
            _target = targetPoint;

            var enemyDatabase = _assetProvider.EnemyDatabase;
            _properties = new Enemy(enemyDatabase.GetEnemy(_enemyOfWave.EnemyId))
            {
                DropChance = _changeCraft
            };

            _health.Initialize(_properties.Health);
            _movement.Initialize(_view, _target, _properties.SpeedMove);
            _attack.Initialize(_view);

            transform.position = spawnPoint.position;

            if (!gameObject.activeSelf)
                gameObject.SetActive(true);

            _view.SetColliderEnabled(true);
            StartMove(_target);
        }

        public void StartMove(Transform _target) => _movement.Move();

        public void StopMove()
        {
            _movement.Stop();
            _view.SetAnimation(AnimationsName.Idle);
        }

        public void ContinueMove()
            => StartMove(_target);

        public bool CheckDistance()
            => _movement.IsInAttackRange(_properties.AttackRadius);

        public virtual void Attack()
        {
            _isAttacked = true;
            _attack.Attack();
        }

        public virtual void Attacked() 
            => WaitNextAttack(_properties.AttackSpeed);

        public void WaitNextAttack(float _delay = -1)
        {
            _isAttacked = false;
            var delay = (_delay < 0) ? _properties.AttackSpeed : _delay;
            _attack.WaitNext(delay, Attack);
        }

        public virtual void GiveDamage()
        {
            if (_barricadeController != null)
                _barricadeController.TakeDamage(_properties.Damage);

            _gameFactory.CreateCollision(CollisionEffectId.Impact, _target.position);
        }

        public void TakeDamage(int _damage) 
            => _health.ApplyDamage(_damage);

        private void OnHealthChanged(float normalized)
        {
            if (_properties != null)
                _properties.Health = _health.Current;
        }

        private void OnHealthDied()
        {
            _collided = true;
            _isDie = true;
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

            _twnFire?.Kill();
        }

        public void Dead()
        {
            _signalBus.Fire(new EnemyDiedSignal(_properties.Id));
            _reward.Grant(_properties);
            CheckSpawnBonus();
            Pop();
        }

        private void CheckSpawnBonus()
        {
            if (UnityEngine.Random.Range(0, 100) <= _properties.DropChance)
            {
                _levelRuntimeService?.SpawnBonus(this.gameObject.transform.position);
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
            _isDie = false;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("BulletHero") && !_collided)
            {
                BaseBulletController bullet = other.GetComponent<BaseBulletController>();
                if (_combatService != null)
                    _combatService.ApplyBulletHit(this, bullet);
                else
                {
                    bullet.Pop();
                    TakeDamage(bullet.Damage);
                }
            }
            else if (other.CompareTag("Grenade") && !_collided)
            {
                GrenadeController grenade = other.GetComponent<GrenadeController>();
                if ((GrenadesName)grenade.Properties.Id == GrenadesName.Grenade_3)
                    grenade.Action(0);
            }

            if (other.CompareTag("Fire"))
            {
                _twnFire = DOVirtual.DelayedCall(1, () =>
                {
                    TakeDamage(1);
                }).SetLoops(-1);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            _twnFire?.Kill();
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
            return component ?? gameObject.AddComponent<T>();
        }
    }
}
