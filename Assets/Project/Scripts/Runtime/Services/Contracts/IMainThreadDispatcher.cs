using System;

namespace SBabchuk.Runtime.Services.Contracts
{
    public interface IMainThreadDispatcher
    {
        void Dispatch(Action callback);
    }
}
