using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using SBabchuk.Runtime.Services.Contracts;
using Zenject;

namespace SBabchuk
{
    public class AssetDataController : MonoBehaviour
    {
        private SkeletonAnimation skltn;

        private GangsterAnimationController gangsterAnimationController;

        private LeaderGangsterController leaderGangsterController;
        private IPlayerProgressService _progressService;

        [Header("Скелетони для різної зброї")]
        public List<SkeletonDataAsset> skeletonDataAsset;

        [Inject]
        private void Construct(IPlayerProgressService progressService)
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

        public void SetAssetData(int _value = 0)
        {
            Debug.Log("SetAssetData: " + _value);
            skltn.skeletonDataAsset = skeletonDataAsset[_value];
            skltn.Initialize(true);

            leaderGangsterController.InitWeapon(_value);

            gangsterAnimationController.Subscribe();
        }
    }
}
