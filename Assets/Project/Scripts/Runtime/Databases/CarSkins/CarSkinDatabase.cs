using System;
using System.Collections.Generic;
using UnityEngine;

namespace SBabchuk.Runtime.Databases.CarSkins
{
    [CreateAssetMenu(menuName = "Databases/Create CarSkinDatabase", fileName = "CarSkinDatabase")]
    public sealed class CarSkinDatabase : ScriptableObject
    {
        [SerializeField] private List<CarSkin> _skins = new();

        public IReadOnlyList<CarSkin> Skins => _skins;

        public CarSkin GetSkin(int id)
            => _skins.Find(skin => skin != null && skin.Id == id);
    }

    [Serializable]
    public sealed class CarSkin
    {
        [SerializeField] private CarSkinsName _skin;
        [SerializeField] private string _name;
        [SerializeField] private Sprite _sprite;
        [SerializeField, Min(0)] private int _price;

        public int Id => (int)_skin;
        public string Name => _name;
        public Sprite Sprite => _sprite;
        public int Price => _price;
    }
}
