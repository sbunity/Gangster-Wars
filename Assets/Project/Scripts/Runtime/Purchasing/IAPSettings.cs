using UnityEngine;

namespace SBabchuk.Runtime.Purchasing
{
    [CreateAssetMenu(menuName = "Purchasing/Create IAPSettings", fileName = "IAPSettings")]
    public sealed class IAPSettings : ScriptableObject
    {
        [SerializeField] private bool _useFakeStore;
        public bool UseFakeStore => _useFakeStore;

        [SerializeField] private FakeStoreMode _fakeStoreMode;
        public FakeStoreMode FakeStoreMode => _fakeStoreMode;

        [SerializeField] private IAPItem[] _storeItems;
        public IAPItem[] StoreItems => _storeItems;
    }
}
