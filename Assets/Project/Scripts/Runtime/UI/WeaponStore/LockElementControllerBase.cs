using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;

namespace SBabchuk
{
    public class LockElementControllerBase : MonoBehaviour
    {
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
