using UnityEngine;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Gameplay.Enemies;
using SBabchuk.Runtime.Services.Contracts;
using Zenject;
using UnityEngine.Serialization;
using SBabchuk.Runtime.Databases.Enemies;
using SBabchuk.Runtime.Databases.Levels;
using SBabchuk.Runtime.Gameplay.Barricades;
using SBabchuk.Runtime.Gameplay.Collisions;

namespace SBabchuk.Runtime.Gameplay.Enemies
{
    [RequireComponent(typeof(EnemyView))]
    [RequireComponent(typeof(EnemyHealth))]
    [RequireComponent(typeof(EnemyMovement))]
    [RequireComponent(typeof(EnemyAttack))]
    [RequireComponent(typeof(EnemyDeath))]
    [RequireComponent(typeof(EnemyReward))]
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

        private const int MaxDropChancePercent = 100;

        private bool _collided;
        private IAssetProvider _assetProvider;
        private IGameFactory _gameFactory;
        private ILevelSpawnService _levelSpawnService;
        private BarricadeController _barricadeController;
        private SignalBus _signalBus;
        private SignalSubscriptions _signals;
        private EnemyView _view;
        private EnemyHealth _health;
        private EnemyMovement _movement;
        private EnemyAttack _attack;
        private EnemyDeath _death;
        private EnemyReward _reward;
        protected ILevelSpawnService LevelSpawnService => _levelSpawnService;
        public bool IsCollided => _collided;

        [Inject]
        public void Construct(IAssetProvider assetProvider, IGameFactory gameFactory, ILevelSpawnService levelSpawnService, BarricadeController barricadeController, SignalBus signalBus)
        {
            _assetProvider = assetProvider;
            _gameFactory = gameFactory;
            _levelSpawnService = levelSpawnService;
            _barricadeController = barricadeController;
            _signalBus = signalBus;
            _signals = new SignalSubscriptions(signalBus)
                .Add<GrenadeDamageSignal>(DealingDamage);
            _signals.Enable();
        }

        private void OnDisable()
        {
            _signals?.Disable();
            StopAllTweens();
        }

        private void OnEnable()
        {
            _signals?.Enable();
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
            _view = GetRequired<EnemyView>();
            _health = GetRequired<EnemyHealth>();
            _movement = GetRequired<EnemyMovement>();
            _attack = GetRequired<EnemyAttack>();
            _death = GetRequired<EnemyDeath>();
            _reward = GetRequired<EnemyReward>();
            _view.Initialize();
            _animation = _view.Animation;
            _center = _view.Center;
            _health.Changed += OnHealthChanged;
            _health.Died += OnHealthDied;
        }

        private void OnDestroy()
        {
            if (_health == null)
                return;

            _health.Changed -= OnHealthChanged;
            _health.Died -= OnHealthDied;
        }

        public virtual void Init(EnemyOfWave _enemyOfWave, Transform spawnPoint, Transform targetPoint, int _changeCraft)
        {
            StopAllTweens();
            _collided = false;
            _isAttacked = false;
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
            if (UnityEngine.Random.Range(0, MaxDropChancePercent) <= _properties.DropChance)
            {
                _levelSpawnService?.SpawnBonus(this.gameObject.transform.position);
            }
        }

        public void Pop()
        {
            this.gameObject.SetActive(false);
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

        private T GetRequired<T>() where T : Component
        {
            var component = GetComponent<T>();
            if (component == null)
                Debug.LogError($"{typeof(T).Name} is required on {name}.", this);

            return component;
        }
    }
}
