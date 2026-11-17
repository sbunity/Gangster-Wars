using System;
using SBabchuk.Runtime.Progress;
using UnityEngine;

namespace SBabchuk.Runtime.Purchasing
{
    [Serializable]
    public sealed class IAPItem
    {
        [SerializeField] private string _androidID;
        [SerializeField] private string _iOSID;

        [SerializeField] private ProductKeyType _productKeyType;
        [SerializeField] private ProductType _productType;

        [SerializeField] private float _defaultUSDPrice = 0.99f;

        private IAPProductSave _save;

        public string ID
        {
            get
            {
#if UNITY_ANDROID
                return _androidID;
#elif UNITY_IOS
                return _iOSID;
#else
                return string.Format("unknown_platform_{0}", _productKeyType);
#endif
            }
        }

        public ProductType ProductType { get => _productType; set => _productType = value; }
        public ProductKeyType ProductKeyType { get => _productKeyType; set => _productKeyType = value; }
        public float DefaultUSDPrice => _defaultUSDPrice;

        public int TimesPurchased => _save.TimesPurchased;

        public void Init(IAPProductSave save)
        {
            _save = save;
        }

        public void OnProductPurchased()
        {
            _save.TimesPurchased++;
        }
    }
}
