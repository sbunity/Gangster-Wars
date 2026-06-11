using UnityEngine;
using DG.Tweening;
using SBabchuk.Runtime.Gameplay.Characters;
using SBabchuk.Runtime.Services.Contracts;
using Zenject;
using UnityEngine.Serialization;

namespace SBabchuk
{
    public class GangsterControllerBase : MonoBehaviour
    {
        [SerializeField, FormerlySerializedAs("e_animation")]
        private GangsterAnimationController _animation;
        public GangsterAnimationController Animation { get => _animation; set => _animation = value; }

        [SerializeField, FormerlySerializedAs("createBulletPoint")]
        private Center _createBulletPoint;
        public Center CreateBulletPoint { get => _createBulletPoint; set => _createBulletPoint = value; }

        [SerializeField, FormerlySerializedAs("twnAtack")]
        private Tween _attackTween;
        public Tween AttackTween { get => _attackTween; set => _attackTween = value; }

        protected IAssetProvider _assetProvider;
        protected IPlayerProgressService _progressService;
        protected IEnemyTargetProvider _enemyTargetProvider;
        protected CharacterView _characterView;
        protected CharacterWeapon _characterWeapon;
        [Inject]
        public void Construct(IAssetProvider assetProvider, IPlayerProgressService progressService, IEnemyTargetProvider enemyTargetProvider)
        {
            _assetProvider = assetProvider;
            _progressService = progressService;
            _enemyTargetProvider = enemyTargetProvider;
        }

        public virtual void Awake()
        {
            _characterView = GetOrAdd<CharacterView>();
            _characterWeapon = GetOrAdd<CharacterWeapon>();
            _characterView.Initialize();
            _animation = _characterView.Animation;
            _createBulletPoint = _characterView.BulletPoint;
        }

        public virtual void Start()
        {
            Init();
        }

        public virtual void Init()
        {
            _animation.SetAnimation(AnimationsName.Idle);
        }

        public virtual void Attack()
        {
        }

        public virtual void AttackEnded()
        {
        }

        public virtual void SpawnBullet()
        {
            Spawn(_createBulletPoint.GetPosition());
        }

        public virtual void Spawn(Vector3 _position)
        {
            _characterWeapon.Fire(0, 4, _position, default(Vector3), 0);
        }

        public virtual void Update()
        {
        }

        private T GetOrAdd<T>()
            where T : Component
        {
            var component = GetComponent<T>();
            return component ?? gameObject.AddComponent<T>();
        }
    }
}
