namespace SBabchuk.Runtime.Purchasing
{
    public enum PurchaseFailureReason
    {
        PurchasingUnavailable,
        ExistingPurchasePending,
        ProductUnavailable,
        SignatureInvalid,
        UserCancelled,
        PaymentDeclined,
        DuplicateTransaction,
        ValidationFailure,
        StoreNotConnected,
        PurchaseMissing,
        Unknown,
    }
}
