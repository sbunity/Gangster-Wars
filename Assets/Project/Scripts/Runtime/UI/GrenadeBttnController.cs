using UnityEngine;
using UnityEngine.UI;
using SBabchuk.Runtime.Services.Contracts;
using Zenject;
using UnityEngine.Serialization;

namespace SBabchuk.Runtime.UI
{
    public class GrenadeBttnController : MonoBehaviour
    {
        [SerializeField, FormerlySerializedAs("grenadesName")]
        private GrenadesName _grenadesName;

        [SerializeField, FormerlySerializedAs("ico")]
        private Image _icon;

        private Image _imgBttn;
        private IHandService _handService;

        [Inject]
        public void Construct(IHandService handService)
        {
            _handService = handService;
        }

        private void Awake()
        {
            _imgBttn = GetComponent<Image>();
        }

        public void PointerDown()
        {
            CheckIco(false);
            _handService?.Init(_grenadesName, this);
        }

        public void CheckIco(bool _value)
        {
            if (_icon.enabled != _value)
                _icon.enabled = _value;

            _imgBttn.raycastTarget = _value;
        }
    }
}
