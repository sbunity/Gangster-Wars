namespace SBabchuk.Runtime.Services.Contracts
{
    public interface IHandService
    {
        bool IsHoldingGrenade { get; }
        void Init(GrenadesName grenadeName, GrenadeBttnController grenadeButton);
    }
}
