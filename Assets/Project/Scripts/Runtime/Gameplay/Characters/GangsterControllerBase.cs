using UnityEngine;
using DG.Tweening;
using SBabchuk.Runtime.Gameplay.Characters;
using SBabchuk.Runtime.Services.Contracts;
using Zenject;

namespace SBabchuk
{
    public class GangsterControllerBase : MonoBehaviour
    {
        [Header("Animator")]
        [HideInInspector]
        public GangsterAnimationController e_animation;

        [Header("Bullet spawn point")]
        [HideInInspector]
        public Center createBulletPoint;

        [HideInInspector]
        public Tween twnAtack;

        protected IAssetProvider AssetProvider;
        protected IPlayerProgressService ProgressService;
        protected IPoolService PoolService;
        protected IGameFactory GameFactory;
        protected ILevelRuntimeService LevelRuntimeService;
        protected CharacterView CharacterView;
        protected CharacterWeapon CharacterWeapon;

        [Inject]
        private void Construct(
            IAssetProvider assetProvider,
            IPlayerProgressService progressService,
            IPoolService poolService,
            IGameFactory gameFactory,
            ILevelRuntimeService levelRuntimeService)
        {
            AssetProvider = assetProvider;
            ProgressService = progressService;
            PoolService = poolService;
            GameFactory = gameFactory;
            LevelRuntimeService = levelRuntimeService;
        }

        public virtual void Awake()
        {
            CharacterView = GetOrAdd<CharacterView>();
            CharacterWeapon = GetOrAdd<CharacterWeapon>();
            CharacterView.Initialize();

            e_animation = CharacterView.Animation;

            createBulletPoint = CharacterView.BulletPoint;
        }

        public virtual void Start()
        {
            Init();
        }

        public virtual void Init()
        {
            e_animation.SetAnimation(AnimationsName.Idle);
        }

        public virtual void Attack()
        {
        }

        public virtual void AttackEnded()
        {
        }

        public virtual void SpawnBullet()
        {
            Spawn(createBulletPoint.GetPosition());
        }

        public virtual void Spawn(Vector3 _position)
        {
            CharacterWeapon.Fire(0, 4, _position, default(Vector3), 0);
        }

        public virtual Pool GetPool(int _spellID)
        {
            if (PoolService != null)
                return PoolService.GetPool(NamesPool.Bullets, _spellID);

            return null;
        }

        public virtual void Update()
        {
        }

        private T GetOrAdd<T>() where T : Component
        {
            var component = GetComponent<T>();
            return component != null ? component : gameObject.AddComponent<T>();
        }
    }
}
