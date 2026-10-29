namespace SBabchuk.Runtime.Services.Contracts
{
    public interface IStoreTabs
    {
        StoreTab Current { get; }
        void Select(StoreTab tab);
    }
}
