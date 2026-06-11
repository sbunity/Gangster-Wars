using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using Zenject;
using UnityEngine.Serialization;

namespace SBabchuk
{
    public class PersonageEnableController : MonoBehaviour
    {
        [SerializeField, FormerlySerializedAs("sniper")]
        private GameObject _sniper;

        [SerializeField, FormerlySerializedAs("bomber")]
        private GameObject _bomber;

        private IPlayerProgressService _progressService;
        
        [Inject]
        public void Construct(IPlayerProgressService progressService)
        {
            _progressService = progressService;
        }

        private void Start()
        {
            _sniper.SetActive(_progressService.GetPersonageShortInfo(0).IsBuy == mySwitch.On);
            _bomber.SetActive(_progressService.GetPersonageShortInfo(1).IsBuy == mySwitch.On);
        }
    }
}
