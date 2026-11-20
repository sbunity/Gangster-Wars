#if MODULE_IAP
using UnityEngine.Purchasing;
#endif

namespace SBabchuk.Runtime.Purchasing
{
    public sealed class ProductData
    {
        private const string DefaultCurrencyCode = "USD";

        public ProductType ProductType { get; }

        public decimal Price { get; }
        public string ISOCurrencyCode { get; }
        public string LocalizedPriceString { get; }

        public bool IsSubscribed { get; }

#if MODULE_IAP
        public Product Product { get; }
#endif

        public ProductData()
        {
            Price = 0.00m;
            ISOCurrencyCode = DefaultCurrencyCode;

            IsSubscribed = false;
        }

        public ProductData(ProductType productType)
        {
            ProductType = productType;

            Price = 0.00m;
            ISOCurrencyCode = DefaultCurrencyCode;

            IsSubscribed = false;
        }

        public string GetLocalPrice()
        {
            return string.Format("{0} {1}", ISOCurrencyCode, Price);
        }

#if MODULE_IAP
        public ProductData(Product product)
        {
            Product = product;

            ProductType = (ProductType)product.definition.type;

            Price = product.metadata.localizedPrice;
            ISOCurrencyCode = product.metadata.isoCurrencyCode;
            LocalizedPriceString = product.metadata.localizedPriceString;
        }
#endif
    }
}
