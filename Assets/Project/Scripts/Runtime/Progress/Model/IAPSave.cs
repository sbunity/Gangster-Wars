using System;
using System.Collections.Generic;
using SBabchuk.Runtime.Purchasing;
using UnityEngine;

namespace SBabchuk.Runtime.Progress
{
    [Serializable]
    public sealed class IAPSave
    {
        [SerializeField] private bool _firstPurchase;
        public bool FirstPurchase { get => _firstPurchase; set => _firstPurchase = value; }

        [SerializeField] private List<IAPProductSave> _products = new();
        public List<IAPProductSave> Products { get => _products; set => _products = value; }

        public IAPProductSave GetProduct(ProductKeyType productKeyType)
        {
            if (_products == null)
                return null;

            foreach (var product in _products)
            {
                if (product != null && product.ProductKeyType == productKeyType)
                    return product;
            }

            return null;
        }
    }
}
