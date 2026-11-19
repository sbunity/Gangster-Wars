using UnityEngine;

namespace SBabchuk.Runtime.Advertisement
{
    [CreateAssetMenu(menuName = "Advertisement/Create AdPlacementConfig", fileName = "AdPlacementConfig")]
    public sealed class AdPlacementConfig : ScriptableObject
    {
        [Header("Level End")]
        [SerializeField, Min(0f)] private float _levelEndInterstitialDelay = 1f;

        [Header("Store")]
        [SerializeField, Min(1)] private int _rewardedCoins = 200;

        public float LevelEndInterstitialDelay => _levelEndInterstitialDelay;
        public int RewardedCoins => _rewardedCoins;
    }
}
