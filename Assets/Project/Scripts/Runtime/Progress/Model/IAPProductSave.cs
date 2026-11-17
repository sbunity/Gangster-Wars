using System;
using SBabchuk.Runtime.Purchasing;
using UnityEngine;

namespace SBabchuk.Runtime.Progress
{
    [Serializable]
    public sealed class IAPProductSave
    {
        [SerializeField] private ProductKeyType _productKeyType;
        public ProductKeyType ProductKeyType => _productKeyType;

        [SerializeField] private int _timesPurchased;
        public int TimesPurchased { get => _timesPurchased; set => _timesPurchased = value; }

        public bool IsPurchased => _timesPurchased > 0;

        public IAPProductSave(ProductKeyType productKeyType)
        {
            _productKeyType = productKeyType;
        }
    }
}
