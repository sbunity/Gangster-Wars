using System;
using System.Collections.Generic;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using Zenject;

namespace SBabchuk.Runtime.Gameplay.Levels
{
    public sealed class LevelCarSkinVariants : MonoBehaviour
    {
        [SerializeField] private List<CarSkinVariant> _variants = new();

        private ICarSkinService _carSkinService;

        [Inject]
        public void Construct(ICarSkinService carSkinService)
            => _carSkinService = carSkinService;

        private void Start()
            => Apply(_carSkinService.SelectedSkinId);

        private void Apply(int skinId)
        {
            if (!_variants.Exists(variant => variant.Id == skinId))
                return;

            foreach (var variant in _variants)
            {
                if (variant.Root != null)
                    variant.Root.SetActive(variant.Id == skinId);
            }
        }

        [Serializable]
        private sealed class CarSkinVariant
        {
            [SerializeField] private CarSkinsName _skin;
            [SerializeField] private GameObject _root;

            public int Id => (int)_skin;
            public GameObject Root => _root;
        }
    }
}
