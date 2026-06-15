using System.Collections.Generic;
using Spine.Unity;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using Zenject;
using UnityEngine.Serialization;

namespace SBabchuk.Runtime.Gameplay.Characters
{
    public class AssetDataController : MonoBehaviour, ILeaderWeaponSelectionService
    {
        [SerializeField, FormerlySerializedAs("skeletonDataAsset")]
        private List<SkeletonDataAsset> _skeletonDataAsset;

        private SkeletonAnimation skltn;
        private GangsterAnimationController gangsterAnimationController;
        private LeaderGangsterController leaderGangsterController;
        private IPlayerProgressService _progressService;
        public WeaponsName CurrentWeapon { get; private set; } = WeaponsName.None;

        [Inject]
        public void Construct(IPlayerProgressService progressService)
        {
            _progressService = progressService;
        }

        private void Awake()
        {
            skltn = GetComponent<SkeletonAnimation>();
            gangsterAnimationController = GetComponent<GangsterAnimationController>();
            leaderGangsterController = GetComponentInParent<LeaderGangsterController>();
        }

        private void Start()
        {
            SetAssetData(_progressService.SelectedWeaponId);
        }

        public void SetAssetData(int value = 0)
        {
            SelectWeapon((WeaponsName)value);
        }

        public void SelectWeapon(WeaponsName weapon)
        {
            var weaponId = (int)weapon;
            if (weaponId < 0 || weaponId >= _skeletonDataAsset.Count)
                return;

            CurrentWeapon = weapon;
            skltn.skeletonDataAsset = _skeletonDataAsset[weaponId];
            skltn.Initialize(true);
            leaderGangsterController.InitWeapon(weaponId);
            gangsterAnimationController.Subscribe();
        }
    }
}
