using System;
using Zenject;

namespace SBabchuk.Runtime.Advertisement
{
    [Serializable]
    public abstract class AdsProviderContainer
    {
        public abstract string ProviderName { get; }
        public abstract AdProviderHandler CreateHandler(IInstantiator instantiator);
    }
}
