namespace SBabchuk.Runtime.Services.Models
{
    public readonly struct StoreItemPrice
    {
        private StoreItemPrice(string label, bool isRealMoney, bool canBuy)
        {
            Label = label;
            IsRealMoney = isRealMoney;
            CanBuy = canBuy;
        }

        public string Label { get; }
        public bool IsRealMoney { get; }
        public bool CanBuy { get; }

        public static StoreItemPrice Unavailable => default;

        public static StoreItemPrice ForCoins(int coins, bool canBuy)
            => new(coins.ToString(), false, canBuy);

        public static StoreItemPrice ForMoney(string label, bool canBuy)
            => new(label, true, canBuy);
    }
}
