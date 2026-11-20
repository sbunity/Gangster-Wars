using System;
using System.Collections.Generic;
using SBabchuk.Runtime.Services.Models;
using UnityEngine;

namespace SBabchuk.Runtime.Purchasing
{
    [CreateAssetMenu(menuName = "Purchasing/Create IAPRewardsConfig", fileName = "IAPRewardsConfig")]
    public sealed class IAPRewardsConfig : ScriptableObject
    {
        [SerializeField] private List<IAPReward> _rewards = new();

        public bool TryGetReward(ProductKeyType product, out IAPReward reward)
        {
            reward = _rewards.Find(entry => entry != null && entry.Product == product);
            return reward != null;
        }

        public bool TryGetProduct(StoreItem item, out ProductKeyType product)
        {
            var reward = _rewards.Find(entry => entry != null && entry.Contains(item));
            product = reward?.Product ?? default;
            return reward != null;
        }
    }

    [Serializable]
    public sealed class IAPReward
    {
        [SerializeField] private ProductKeyType _product;
        [SerializeField, Min(0)] private int _coins;
        [SerializeField] private WeaponsName _weapon = WeaponsName.None;
        [SerializeField] private PersonagesName _personage = PersonagesName.None;
        [SerializeField] private DefencesName _defence = DefencesName.None;
        [SerializeField] private CarSkinsName _carSkin = CarSkinsName.None;
        [SerializeField] private bool _removeAds;

        public ProductKeyType Product => _product;
        public int Coins => _coins;
        public bool RemovesAds => _removeAds;

        public IEnumerable<StoreItem> Items
        {
            get
            {
                if (_weapon != WeaponsName.None)
                    yield return StoreItem.Weapon((int)_weapon);

                if (_personage != PersonagesName.None)
                    yield return StoreItem.Personage((int)_personage);

                if (_defence != DefencesName.None)
                    yield return StoreItem.Defence((int)_defence);

                if (_carSkin != CarSkinsName.None)
                    yield return StoreItem.CarSkin((int)_carSkin);
            }
        }

        public bool Contains(StoreItem item)
        {
            foreach (var rewardItem in Items)
            {
                if (rewardItem.Equals(item))
                    return true;
            }

            return false;
        }
    }
}
