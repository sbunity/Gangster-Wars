using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using Zenject;

namespace SBabchuk
{
    public class LockElementControllerBase : MonoBehaviour
    {
        protected IAssetProvider _assetProvider;
        protected IPlayerProgressService _progressService;

        [Inject]
        public void Construct(IAssetProvider assetProvider, IPlayerProgressService progressService)
        {
            _assetProvider = assetProvider;
            _progressService = progressService;
        }

        [SerializeField, FormerlySerializedAs("priceBuy")]
        private Text _priceBuy;
        public Text PriceBuy { get => _priceBuy; set => _priceBuy = value; }

        [SerializeField, FormerlySerializedAs("bttnBuy")]
        private Button _bttnBuy;
        public Button BttnBuy { get => _bttnBuy; set => _bttnBuy = value; }

        [SerializeField, FormerlySerializedAs("id")]
        private int _id;
        public int Id { get => _id; set => _id = value; }

        public virtual void Initialisation(int _id)
        {
        }

        public virtual void Buy()
        {
        }
    }
}
