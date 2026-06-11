using System.Collections;
using System.Collections.Generic;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using Zenject;

namespace SBabchuk
{ 
    public class PersonageEnableController : MonoBehaviour
    {
        [Header("Снайпер")]
        public GameObject sniper;

        [Header("Бомбер")]
        public GameObject bomber;
        private IPlayerProgressService _progressService;

        [Inject]
        private void Construct(IPlayerProgressService progressService)
        {
            _progressService = progressService;
        }

        private void Start()
        {
            sniper.SetActive(_progressService.GetPersonageShortInfo(0).isBuy == mySwitch.On);

            bomber.SetActive(_progressService.GetPersonageShortInfo(1).isBuy == mySwitch.On);
        }
    }
}
