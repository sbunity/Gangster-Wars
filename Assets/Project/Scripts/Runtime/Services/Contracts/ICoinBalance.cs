using System;

namespace SBabchuk.Runtime.Services.Contracts
{
    public interface ICoinBalance
    {
        int Coins { get; }

        event Action<int> Changed;
    }
}
