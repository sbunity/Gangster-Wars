using System.Collections.Generic;
using Spine.Unity;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using Zenject;
using UnityEngine.Serialization;

namespace SBabchuk
{
    public class AssetDataController : MonoBehaviour
    {
        [SerializeField, FormerlySerializedAs("skeletonDataAsset")]
        private List<SkeletonDataAsset> _skeletonDataAsset;

        private SkeletonAnimation skltn;
        private GangsterAnimationController gangsterAnimationController;
        private LeaderGangsterController leaderGangsterController;
        private IPlayerProgressService _progressService;

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

        private void SetAssetData(int value = 0)
        {
            skltn.skeletonDataAsset = _skeletonDataAsset[value];
            skltn.Initialize(true);
            leaderGangsterController.InitWeapon(value);
            gangsterAnimationController.Subscribe();
        }
    }
}
