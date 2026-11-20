using System;

namespace SBabchuk.Runtime.Services.Models
{
    public readonly struct StoreItem : IEquatable<StoreItem>
    {
        public StoreItem(StoreItemType type, int id)
        {
            Type = type;
            Id = id;
        }

        public StoreItemType Type { get; }
        public int Id { get; }

        public static StoreItem Weapon(int id) => new(StoreItemType.Weapon, id);
        public static StoreItem Personage(int id) => new(StoreItemType.Personage, id);
        public static StoreItem Defence(int id) => new(StoreItemType.Defence, id);
        public static StoreItem CarSkin(int id) => new(StoreItemType.CarSkin, id);

        public bool Equals(StoreItem other)
            => Type == other.Type && Id == other.Id;

        public override bool Equals(object obj)
            => obj is StoreItem other && Equals(other);

        public override int GetHashCode()
            => ((int)Type * 397) ^ Id;

        public override string ToString()
            => $"{Type} {Id}";
    }
}
